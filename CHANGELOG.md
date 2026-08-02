# Changelog

All notable changes to this project will be documented in this file. See [versionize](https://github.com/saintedlama/versionize) for commit guidelines.

## [0.5.0] - 2026-08-02

### Breaking

- Retargeted to .NET 10. Consumers on .NET 9 or earlier need to stay on 0.4.2.
- `U+0000` is now rejected by the prohibited value step. It is prohibited by RFC 3454 table
  C.2.1 but was previously allowed through, so input that used to pass may now throw.
- Characters outside the Basic Multilingual Plane are no longer rejected as surrogate code
  points by table C.5. A well formed surrogate pair encodes a legal supplementary character
  and now passes, where it previously threw `ProhibitedValueException`.
- `ProhibitedValueException.Message` reports the code point as `U+XXXX` rather than quoting
  the character, so that non-printable values are legible.

### Fixed

- The mapping, prohibited value and bidirectional steps walked their input one UTF-16 code
  unit at a time, so they could never match the table entries above `U+FFFF` — 524 of the
  case folding keys in B.2, 37 of the ranges in D.2, and 16 of the 18 in C.4. All three steps
  now iterate by code point.
- `MappingStep` threw `OverflowException` on any replacement above `U+FFFF`, which was
  reachable through the shipped B.2 and B.3 tables.
- `MappingTableBuilder.Remove` passed single code points to the value range compiler, which
  expects start/end pairs. An odd number of removals threw `ArgumentException`, and an even
  number was silently misread as ranges.
- `ValueRangeCompiler` applied only the first removal and then either ignored the rest or ran
  off the end of the list.

### Added

- `ProhibitedValueException.CodePoint` exposes the offending code point, and a constructor
  taking an `int` for values that do not fit in a `char`.

### Changed

- Package versions are managed centrally in `Directory.Packages.props`.
- Releases publish to nuget.org through NuGet trusted publishing, triggered by a `v*` tag.
- Documentation moved from an unwritten Docusaurus scaffold to a DocFX site.

## [0.3.2] - 2020-12-03

### Added

- Added documentation site

## [0.3.1] - 2020-11-26

### Changed

- Resolved issue with CI package release

## [0.3.0] - 2019-12-03

### Added

- Added tests for all public classes

### Changed

- Refactored generated code to latest C# 9 features

## 0.2.2 (2019-1-25)

## 0.2.1 (2019-1-25)

### Bug Fixes

- add parentheses for precedence clarity

## 0.2.0 (2019-1-24)
