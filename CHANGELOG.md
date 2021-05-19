# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [vNext]

## [1.1.1] - 2021-05-17

### Added
- `ConnectedComponent` and `ConnectedComponentBase` as base classes for connected components.
- Page can be connected via `ConnectedComponentBase`.
- Changelog file

### Changed
- Fixed AsyncActions not logged to Redux DevTools.
- Fixed connected component render logic to not do unnecessary renders
- Updated dependencies to the latest versions.

### Removed
- `ComponentConnector` class superseded by `ConnectedComponent` and `ConnectedComponentBase`.

[VNext]: https://github.com/BerserkerDotNet/BlazorState/compare/v1.1.1...vNext
[1.1.1]: https://github.com/BerserkerDotNet/BlazorState/compare/v1.0.4...v1.1.1