# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [1.0.1] - 2026-04-01

### Changed

- Set `hideFlags` to prevent reset on non-referenced assets.
- Commits .meta files. Required by Unity when importing as immutable package.

## [1.0.0] - 2025-09-21

### Added
- Initial implementation of all core SOAR systems
  - `SoarCore`
  - `Command`
  - `GameEvent`
  - `Variable`
  - `Collection`
  - `Transaction`
  - etc.
- [R3] integration for respective features where applicable.
- A set of ready-to-use Base classes for common types.
- Custom Inspector layouts for improved usability.
- A full documentation website with English and Japanese translations.
- Sample scenes and scripts for each feature.

[Unreleased]: https://github.com/ripandy/SOAR/compare/1.0.1...HEAD
[1.0.1]: https://github.com/ripandy/SOAR/compare/1.0.0...1.0.1
[1.0.0]: https://github.com/ripandy/SOAR/releases/tag/1.0.0
[R3]: https://github.com/Cysharp/R3