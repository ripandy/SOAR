# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Changed

- `Collection.Copy()`, `ResetValues()` and `FromJsonString()` now share one replace behavior:
  `OnClear` (only if the collection was not empty), then `OnAdd` for each element, then `Count` once, only if it changed.
  Previously `Copy()` cleared without raising `OnClear`, and `FromJsonString()` raised `Count` twice (0, then N).
- `Variable` and `Collection` compare values with `EqualityComparer<T>.Default`, avoiding a boxing allocation on every assignment.
- `SoarDictionary` updates its key lookup before raising add events, so handlers always see consistent state. `Count` is now raised last.

### Fixed

- Disposing a subscription from inside its own handler no longer throws during `Raise()` (without R3).
- Disposing a list of subscriptions no longer skips every other element (without R3).
- `Copy()` from the collection itself, or a lazy view over it, no longer empties the collection.
- `SoarDictionary`: assigning a missing key through the indexer now adds it instead of throwing.
- `SoarDictionary.AddRange()`: a duplicate key now throws before anything changes,
  and an `OnAdd` handler that reads the dictionary no longer causes an exception.
- `Transaction`: when a registered response throws, `RequestAsync()` now fails with that exception instead of never completing.
  Exceptions from callback requests are logged instead of being silently discarded.
- Collection sample handles `OnClear`, so it no longer shows stale rows after `Clear()`, `Copy()` or loading.

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