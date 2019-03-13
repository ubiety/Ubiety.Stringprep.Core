# ![Logo](https://github.com/ubiety/Ubiety.Stringprep.Core/raw/develop/images/rope64.png) Ubiety.Stringprep.Core ![Nuget](https://img.shields.io/nuget/v/Ubiety.Stringprep.Core.svg?style=flat-square)

> Ubiety Stringprep is an implementation of RFC 3454 for .NET Core.

| Branch  | Quality                                                                                                                                                                                                                | Travis CI | Appveyor                                                                                                              | Coverage                                                                                                                                                                     |
| ------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | --------- | --------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------- |
| Master  | [![CodeFactor](https://www.codefactor.io/repository/github/ubiety/ubiety.stringprep.core/badge?style=flat-square)](https://www.codefactor.io/repository/github/ubiety/ubiety.stringprep.core)                          |           | ![AppVeyor branch](https://img.shields.io/appveyor/ci/coder2000/ubiety-stringprep-core/master.svg?style=flat-square)  | ![Coveralls github branch](https://img.shields.io/coveralls/github/ubiety/Ubiety.Stringprep.Core/master.svg?style=flat-square)  |
| Develop | [![CodeFactor](https://www.codefactor.io/repository/github/ubiety/ubiety.stringprep.core/badge/develop?style=flat-square)](https://www.codefactor.io/repository/github/ubiety/ubiety.stringprep.core/overview/develop) |           | ![AppVeyor branch](https://img.shields.io/appveyor/ci/coder2000/ubiety-stringprep-core/develop.svg?style=flat-square) | ![Coveralls github branch](https://img.shields.io/coveralls/github/ubiety/Ubiety.Stringprep.Core/develop.svg?style=flat-square) |

## Installing / Getting started

Ubiety Stringprep Core is available from NuGet

```shell
dotnet package install Ubiety.Stringprep.Core
```

You can also use your favorite NuGet client.

## Developing

Here's a brief intro about what a developer must do in order to start developing
the project further:

```shell
git clone https://github.com/ubiety/Ubiety.Stringprep.Core.git
cd Ubiety.Stringprep.Core
dotnet restore
```

Clone the repository and then restore the development requirements. You can use
any editor, Rider, VS Code or VS 2017. The library supports all .NET Core
platforms.

### Building

Building is simple

```shell
./build.ps1
```

### Deploying / Publishing

```shell
git pull
versionize
dotnet pack
dotnet nuget push
git push
```

## Contributing

When you publish something open source, one of the greatest motivations is that
anyone can just jump in and start contributing to your project.

These paragraphs are meant to welcome those kind souls to feel that they are
needed. You should state something like:

"If you'd like to contribute, please fork the repository and use a feature
branch. Pull requests are warmly welcome."

If there's anything else the developer needs to know (e.g. the code style
guide), you should link it here. If there's a lot of things to take into
consideration, it is common to separate this section to its own file called
`CONTRIBUTING.md` (or similar). If so, you should say that it exists here.

## Links

- Project homepage: <https://stringprep.ubiety.ca>
- Repository: <https://github.com/ubiety/Ubiety.Stringprep.Core/>
- Issue tracker: <https://github.com/ubiety/Ubiety.Stringprep.Core/issues>
  - In case of sensitive bugs like security vulnerabilities, please use the 
    [Tidelift security contact](https://tidelift.com/security) instead of using issue tracker. 
    We value your effort to improve the security and privacy of this project! Tidelift will coordinate the fix and disclosure.
- Related projects:
  - Ubiety VersionIt: <https://github.com/ubiety/Ubiety.VersionIt/>
  - Ubiety Toolset: <https://github.com/ubiety/Ubiety.Toolset/>
  - Ubiety Xmpp: <https://github.com/ubiety/Ubiety.Xmpp.Core/>
  - Ubiety Dns: <https://github.com/ubiety/Ubiety.Dns.Core/>

## Sponsors

### Gold Sponsors

[![Gold Sponsors](https://opencollective.com/ubiety/tiers/gold-sponsor.svg?avatarHeight=36)](https://opencollective.com/ubiety/)

### Silver Sponsors

[![Silver Sponsors](https://opencollective.com/ubiety/tiers/silver-sponsor.svg?avatarHeight=36)](https://opencollective.com/ubiety/)

### Bronze Sponsors

[![Bronze Sponsors](https://opencollective.com/ubiety/tiers/bronze-sponsor.svg?avatarHeight=36)](https://opencollective.com/ubiety/)

## Licensing

The code in this project is licensed under the [Unlicense](https://unlicense.org/)