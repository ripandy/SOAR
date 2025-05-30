# Installation

## OpenUPM 

__Import via scoped registry. Update from Package Manager.__

To add OpenUPM to your project:

- open `Edit/Project Settings/Package Manager`
- add a new Scoped Registry:
```
Name: OpenUPM
URL:  https://package.openupm.com/
Scope(s):
  - com.ripandy
  - com.cysharp.r3 (Recommended)
```
- click Save
- Open `Window/Package Manager`
- Select `My Registries` in top left dropdown
- Select `SOAR` and click `Install`
- Select `R3` and click `Install` (Recommended) (see: Note)

!!! note
    Installation for [R3] requires dependency imports from NuGet. See [R3 Unity Installation](https://github.com/Cysharp/R3?tab=readme-ov-file#unity) for further detail.

## GitHub URL

__Use the GitHub link for importing. Update using the Package Manager in Unity 2021.2 or later.__

SOAR can be added directly from GitHub.
From Unity 2021.2, Package Manager can be used to update to latest version on the main branch.
Otherwise, you need to update manually by removing and then adding back the package.

- Open the Package Manager
- Click the `+` icon
- Select the `Add from Git URL` option
- Paste the following URL: `https://github.com/ripandy/SOAR.git`
- Click `Add`

To install specific version, refer to SOAR's release tags.
For example: `https://github.com/ripandy/SOAR.git#1.0.0`

## Clone to Local Folder

__Clone SOAR to local folder as independent project.__

SOAR can be cloned to local folder and can be treated as independent project.
However, SOAR requires separate Unity Project in order to test its functionality.

- Clone SOAR's repository to your local directory.
- Open containing Unity project to import SOAR.
- Open `Window/Package Manager`
- Click the `+` icon
- Select the `Install package from disk` option
- Select the `package.json` file from the cloned directory.
- Click `Add`

The package will be added to manifest.json as a local package (`file://`).
Source codes can then be modified from containing Unity Project.
Changes can be managed with git as usual.
Package path can be changed to relative path as an alternative to the default absolute path.

```json
{
  "dependencies": {
    "com.ripandy.soar": "file:../path/to/your/local/SOAR"
  }
}
```

## Clone to Packages Folder

Clone this repository to Unity Project's Packages directory: `YourUnityProject/Packages/`.

Unity will treat the project as a custom package.
Source codes can then be modified from containing Unity Project.
Changes can be managed with git as usual.
SOAR can also be cloned as Submodule of your main git repository.


[SOAR]: https://github.com/ripandy/SOAR
[R3]: https://github.com/Cysharp/R3
