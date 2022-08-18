using System.Collections.Generic;
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
using Nuke.Common.Tools.SonarScanner;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using Nuke.Common.Utilities;
using static Nuke.Common.ChangeLog.ChangelogTasks;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.SonarScanner.SonarScannerTasks;

[GitHubActions(
    "release",
    GitHubActionsImage.WindowsLatest,
    OnPushBranches = new[] { MasterBranch, ReleaseBranchPrefix + "/*"},
    InvokedTargets = new[] { nameof(Test), nameof(Publish) },
    ImportSecrets = new[] { nameof(NuGetKey) },
    EnableGitHubToken = true)]
[GitHubActions(
    "continuous",
    GitHubActionsImage.WindowsLatest, 
    GitHubActionsImage.UbuntuLatest,
    GitHubActionsImage.MacOsLatest,
    OnPushBranchesIgnore = new[] { MasterBranch, ReleaseBranchPrefix + "/*"},
    OnPullRequestBranches = new[] { DevelopBranch },
    PublishArtifacts = false,
    InvokedTargets = new[] { nameof(Test), nameof(Publish) },
    EnableGitHubToken = true)]
[AppVeyor(
    AppVeyorImage.UbuntuLatest,
    AppVeyorImage.VisualStudioLatest,
    InvokedTargets = new[] { nameof(Test), nameof(SonarEnd)},
    SkipTags = true,
    AutoGenerate = true)]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Required] [GitRepository] readonly GitRepository GitRepository;
    [Required] [GitVersion(Framework = "netcoreapp3.1")] readonly GitVersion GitVersion;
    [Required] [Solution] readonly Solution Solution;

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
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
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
                .SetVersion(GitVersion.NuGetVersionV2)
                .SetOpenCoverPaths(ArtifactsDirectory / "coverage.opencover.xml")
                .SetProcessArgumentConfigurator(args => args.Add("/o:ubiety"))
                .SetFramework("net5.0"));
        });

    Target SonarEnd => _ => _
        .After(Test)
        .DependsOn(SonarBegin)
        .AssuredAfterFailure()
        .Unlisted()
        .Executes(() =>
        {
            SonarScannerEnd(_ => _
                .SetFramework("net5.0"));
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
                    .SetProcessArgumentConfigurator(args => args.Add("/p:Exclude={0}", "[xunit.*]*"))));
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
                .SetVersion(GitVersion.NuGetVersionV2)
                .SetPackageReleaseNotes(GetNuGetReleaseNotes(ChangelogFile, GitRepository)));
        });

    [Parameter] readonly string NuGetKey;
    string NuGetSource => "https://api.nuget.org/v3/index.json";
    IEnumerable<AbsolutePath> PackageFiles => ArtifactsDirectory.GlobFiles("*.nupkg");
    string GitHubSource => $"https://nuget.pkg.github.com/{GitHubActions.RepositoryOwner}/index.json";
    bool Beta => GitRepository.IsOnDevelopBranch() || GitRepository.IsOnFeatureBranch();

    string Source => Beta ? GitHubSource : NuGetSource;

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
                    .SetPassword(GitHubToken));
            }
            
            DotNetNuGetPush(_ => _
                    .SetApiKey(NuGetKey)
                    .SetSource(Source)
                    .CombineWith(PackageFiles, (_, v) => _
                        .SetTargetPath(v)),
                5,
                true);
        });

    public static int Main() => Execute<Build>(x => x.Test);
}
