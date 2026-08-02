using JetBrains.Annotations;
using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.CI.AppVeyor;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Tools.SonarScanner;
using Nuke.Common.Utilities;
using Nuke.Common.Utilities.Collections;
using System.Collections.Generic;
using static Nuke.Common.ChangeLog.ChangelogTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.SonarScanner.SonarScannerTasks;

// Releases go to nuget.org via trusted publishing, which needs id-token to exchange the OIDC
// token for a short-lived key. A single image keeps the push from racing itself.
//
// The tag is the trigger, not the merge into master: GitVersion only resolves the release version
// once the tag exists, so a branch push would publish whatever build-metadata version it computed
// for the untagged merge commit.
[CustomGitHubActions(
    "release",
    GitHubActionsImage.UbuntuLatest,
    OnPushTags = ["v*"],
    PublishArtifacts = false,
    InvokedTargets = [nameof(Test), nameof(Publish)],
    ReadPermissions = [GitHubActionsPermissions.Contents],
    WritePermissions = [GitHubActionsPermissions.IdToken],
    FetchDepth = 0)]
// Pre-releases go to GitHub Packages, so this workflow needs the GitHub token and nothing from
// nuget.org. Declaring any permission drops the rest to none, hence contents for the checkout.
[GitHubActions(
    "continuous",
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    GitHubActionsImage.MacOsLatest,
    OnPushBranchesIgnore = [MasterBranch, ReleaseBranchPrefix + "/*"],
    PublishArtifacts = false,
    InvokedTargets = [nameof(Test), nameof(Publish)],
    EnableGitHubToken = true,
    ReadPermissions = [GitHubActionsPermissions.Contents],
    WritePermissions = [GitHubActionsPermissions.Packages],
    FetchDepth = 0)]
// Pull requests test only. A pull_request event checks out the detached merge ref, so there is no
// branch for GitRepository to resolve, Beta is false whatever the source branch is called, and
// Publish would demand a nuget.org key this workflow has no reason to hold. Keeping Publish out
// of the pull request build lets it keep a hard Requires for the release path.
[GitHubActions(
    "pr",
    GitHubActionsImage.WindowsLatest,
    GitHubActionsImage.UbuntuLatest,
    GitHubActionsImage.MacOsLatest,
    OnPullRequestBranches = [DevelopBranch, MasterBranch],
    PublishArtifacts = false,
    InvokedTargets = [nameof(Test)],
    ReadPermissions = [GitHubActionsPermissions.Contents],
    FetchDepth = 0)]
[AppVeyor(
    AppVeyorImage.VisualStudioLatest,
    InvokedTargets = [nameof(Test), nameof(SonarEnd)],
    SkipTags = true,
    AutoGenerate = true)]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Required][GitRepository] readonly GitRepository GitRepository;
    [Required][GitVersion] readonly GitVersion GitVersion;
    [Required][Solution] readonly Solution Solution;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    const string MasterBranch = "master";
    const string DevelopBranch = "develop";
    const string ReleaseBranchPrefix = "release";

    [Parameter] readonly string GitHubToken;

    [CI] GitHubActions GitHubActions;

    [UsedImplicitly]
    Target Clean => _ => _
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").DeleteDirectories();
            TestsDirectory.GlobDirectories("**/bin", "**/obj").DeleteDirectories();
            ArtifactsDirectory.CreateOrCleanDirectory();
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(_ => _
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(_ => _
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .SetNoRestore(InvokedTargets.Contains(Restore)));
        });

    const string SonarProjectKey = "ubiety_Ubiety.Stringprep.Core";

    Target SonarBegin => _ => _
        .Before(Compile)
        .Unlisted()
        .Executes(() =>
        {
            SonarScannerBegin(_ => _
                .SetProjectKey(SonarProjectKey)
                .SetServer("https://sonarcloud.io")
                .SetVersion(GitVersion.NuGetVersion)
                .SetOpenCoverPaths(ArtifactsDirectory / "coverage.opencover.xml")
                .SetOrganization("ubiety")
                .SetFramework("net10.0"));
        });

    Target SonarEnd => _ => _
        .After(Test)
        .DependsOn(SonarBegin)
        .AssuredAfterFailure()
        .Unlisted()
        .Executes(() =>
        {
            SonarScannerEnd(_ => _
                .SetFramework("net10.0"));
        });

    [Parameter] readonly bool Cover = true;
    Project TestProject => Solution.GetProject("Ubiety.Stringprep.Tests");

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
        {
            DotNetTest(_ => _
                .SetProjectFile(TestProject)
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .SetConfiguration(Configuration)
                .When(Cover, _ => _
                    .EnableCollectCoverage()
                    .SetCoverletOutputFormat(CoverletOutputFormat.opencover)
                    .SetCoverletOutput(ArtifactsDirectory / "coverage")
                    .SetProcessAdditionalArguments("/p:Exclude=\"[xunit.*]*\"")));
        });

    string ChangelogFile => RootDirectory / "CHANGELOG.md";

    Target Pack => _ => _
        .DependsOn(Compile)
        .After(Test)
        .Produces(ArtifactsDirectory / "*.nupkg")
        .Executes(() =>
        {
            DotNetPack(_ => _
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .SetConfiguration(Configuration)
                .SetOutputDirectory(ArtifactsDirectory)
                .SetVersion(GitVersion.NuGetVersion)
                .SetPackageReleaseNotes(GetNuGetReleaseNotes(ChangelogFile, GitRepository)));
        });

    [Parameter] readonly string NuGetKey;
    string NuGetSource => "https://api.nuget.org/v3/index.json";
    IEnumerable<AbsolutePath> PackageFiles => ArtifactsDirectory.GlobFiles("*.nupkg");
    string GitHubSource => $"https://nuget.pkg.github.com/{GitHubActions.RepositoryOwner}/index.json";
    bool Beta => GitRepository.IsOnDevelopBranch() || GitRepository.IsOnFeatureBranch();

    string Source => Beta ? GitHubSource : NuGetSource;
    string ApiKey => Beta ? GitHubToken : NuGetKey;

    Target Publish => _ => _
        .DependsOn(Pack)
        .Consumes(Pack)
        .Requires(() => !NuGetKey.IsNullOrEmpty() || Beta)
        .Requires(() => Configuration.Equals(Configuration.Release))
        .Executes(() =>
        {
            if (Beta)
            {
                DotNetNuGetAddSource(_ => _
                    .SetSource(GitHubSource)
                    .SetUsername(GitHubActions.Actor)
                    .SetPassword(GitHubToken)
                    .SetStorePasswordInClearText(true));
            }

            // The continuous workflow runs this target on every image in the matrix, and they all
            // compute the same version, so whichever job gets there first wins and the rest see
            // 409 Conflict. Skipping duplicates makes the push idempotent instead of a race.
            DotNetNuGetPush(_ => _
                    .SetApiKey(ApiKey)
                    .SetSource(Source)
                    .EnableSkipDuplicate()
                    .CombineWith(PackageFiles, (_, v) => _
                        .SetTargetPath(v)),
                5,
                true);
        });

    public static int Main() => Execute<Build>(x => x.Test);
}
