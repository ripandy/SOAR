# Scriptable Object Architecture Reactive-extensible

<!-- [![openupm](https://img.shields.io/npm/v/com.kadinche.kassets?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.kadinche.kassets/) -->

SOAR is an implementation of [Scriptable Object](https://docs.unity3d.com/Manual/class-ScriptableObject.html) Architecture.
Scriptable Object Architecture intends to provide clean and decoupled code architecture.
SOAR is implemented based on [Ryan Hipple's talk at Unite Austin 2017](https://youtu.be/raQ3iHhE_Kk).

SOAR is developed and designed to be extensible with Reactive library R3.
R3 is a feature rich and modern Reactive Extensions for C#.
SOAR tries to wraps and utilize R3's feature within the Scriptable Object Architecture.
SOAR can function independently, but its implementation is bare minimum.
It is highly recommended to use SOAR in conjunction with R3.

- [R3] - The new future of dotnet/reactive and UniRx.
- [Kassets] - SOAR's predecessor. Scriptable Object Architecture extensible with UniRx and UniTask.

### Unity Version
- Unity 2022.3+
- Note that this GitHub project cannot be opened directly in Unity Editor. See [Installation](https://github.com/kadinche/Kassets#Installation) for cloning.

<!-- __For further details, see [Documentation]__ -->

# Getting Started

## Installation

<details>

<summary>Add from OpenUPM | <em>Import via scoped registry.</em></summary>

To add OpenUPM to your project:

- open `Edit/Project Settings/Package Manager`
- add a new Scoped Registry:
```
Name: OpenUPM
URL:  https://package.openupm.com/
Scope(s):
  - com.ripandy
  - com.cysharp.r3 (optional)
```
- click <kbd>Save</kbd>
- Open `Window/Package Manager`
- Select ``My Registries`` in top left dropdown
- Select ``SOAR`` and click ``Install``
- Select ``R3`` and click ``Install`` (Optional) (see: Note)

> [!NOTE]
> Installation of [R3] in Unity requires installation from NuGet. See [R3 Unity Installation](https://github.com/Cysharp/R3?tab=readme-ov-file#unity) for further detail.

</details>

<details>

<summary>Add from GitHub | <em>Use github link to import.</em></summary>

Add Package directly from GitHub.

- Open `Window/Package Manager`
- Click the `+` icon
- Select the `Add package from Git URL` option
- Paste the following URL: `https://github.com/ripandy/SOAR.git`
- Click `Add`

To install specific version, refer to SOAR's release tags.
For example: `https://github.com/ripandy/SOAR.git#1.0.0`

</details>

<details>

<summary>Clone to Packages Folder | <em>For development or contribution purpose.</em></summary>

Clone this repository to Unity Project's Packages directory.
Unity will treat the project as a package.
Modify source codes from containing Unity Project.
Manage changes with git as usual.
SOAR can also be cloned as Submodule.

- Clone SOAR's git to `YourUnityProject/Packages/` folder

</details>

# References:
- [Unite2017 Scriptable Object Architecture sample project](https://github.com/roboryantron/Unite2017)
- [Unite2017 Game Architecture with Scriptable Objects on Slideshare](https://www.slideshare.net/RyanHipple/game-architecture-with-scriptable-objects)
- [R3 — A New Modern Reimplementation of Reactive Extensions for C#](https://neuecc.medium.com/r3-a-new-modern-reimplementation-of-reactive-extensions-for-c-cf29abcc5826)
- [Kassets (SOAR's predecessor). Scriptable Object Architecture extensible with UniRx and UniTask](https://github.com/kadinche/Kassets).

# LICENSE

- SOAR is Licensed under [MIT License](LICENSE)
- [R3] is Licensed under [MIT License](https://github.com/Cysharp/R3/blob/main/LICENSE)
- [Kassets] (SOAR's predecessor) is Licensed under [MIT License](https://github.com/kadinche/Kassets/blob/main/LICENSE)

[R3]: https://github.com/Cysharp/R3
[Kassets]: https://github.com/kadinche/Kassets
[UniRx]: https://github.com/kadinche/UniRx
[UniTask]: https://github.com/Cysharp/UniTask
[Documentation]: https://Kadinche.github.io/Kassets/