# Humanoid 3D Models for IVH SDK
All notable changes to this package are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/)
and this package adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.1.0] - 2026-09-15

### Removed
- **Breaking:** All Presence Team character models and prefabs (`Runtime/Models/PresenceTeamCharacters`, `Runtime/Prefabs/PresenceTeam`). Scenes referencing those prefabs will lose them.
- Baked lighting data for a scene that is no longer part of the package (`Runtime/Scenes/Main`).
- Unreferenced duplicate of the environment mesh and material (`Runtime/Environment`).
- Orphaned `GlobalVolumeFeature` renderer feature from `Mobile_High_Renderer.asset`; both its script and its volume profile were missing.
- **All third-party-derived hair content.** The Business Female and Business Male meshes were re-exported with replacement hair using Didimo's own hair sets; the superseded texture sets were deleted, and the third-party hair shader and its subgraph were removed from `Runtime/Shader`.

### Added
- `Example Scene` sample, importable from the Package Manager. Contains the demo scene with every character prefab, its baked lighting and reflection probes, the environment mesh and the skybox.
- `Per-Camera Render POCs` sample: three reference scenes, each hiding a humanoid's head from its own first-person camera by a different technique (layer split with a duplicate renderer, per-camera material swap, and a custom URP render feature). Reference material rather than a runnable demo. Ships with a README covering each method's trade-offs and the two project-level setup steps it needs: the four avatar layers used by method 1, and registering the renderer feature used by method 3.
- The POC sample draws its characters, materials and shaders from the package itself rather than carrying duplicate copies. Its avatars are prefab variants of `Runtime/Prefabs/Business Male` and `Business Female`, and its materials reference the graphs in `Runtime/Shader`. This removes 347 MB of duplicated models and textures from the package, and with it the "multiple shaders share the name" warnings that two copies of the same Shader Graph produced.
- Explicit dependency on `com.unity.render-pipelines.universal` (14.0.12), which the character materials and shaders have always required.
- `Documentation~/index.md` with installation, Git LFS, render pipeline setup and sample import instructions.
- Package keywords and sample metadata in `package.json`.
- Standalone documentation pages under `Documentation~/`:
  - `Prefabs.md` and `CullMasks.md`, moved out of `README.md`.
  - `FirstPersonVRCameras.md`, the three head-hiding techniques behind the
    `Per-Camera Render POCs` sample.
  - `AnimationRetargeting.md`, Reallusion/Mixamo to PRESENCE retargeting, with reference
    screenshots.
  - `AddingCustomDeformables.md`, fitting custom deformables, with reference
    screenshots.
  - `BlendshapeRenaming.md`, making Rocketbox facial blend shapes compatible with Reallusion,
    including the renaming script and the full target mapping.
  - `PackageContents.md`, every use case character shipped, cross-referenced against the WP5
    tracking sheet.
  - `FAQ.md`, current limitations: no runtime character loading, Unity Humanoid compatibility,
    mesh and facial blend shape specifications, and adding garments.
- `Third Party Notices.md`, documenting that URP is an external dependency, not bundled
  content, and that the package bundles no other third-party assets.
- A rewritten `LICENSE.md`: the character assets get a permissive license (usable commercially
  or non-commercially, inside or outside PRESENCE, attribution required), plus a liability
  disclaimer and a link to Didimo's privacy policy.
- EU funding acknowledgement (Horizon Europe grant agreement No 101135025, PRESENCE project) in
  `README.md` and `LICENSE.md`.
- The package's GitHub URL (`github.com/didimoinc/ivh-sdk-humanoids`) in `package.json`'s
  `repository` field, `README.md`, and the install snippet in `Documentation~/index.md`.

### Changed
- **Display name** changed from `Intelligent Virtual Human SDK Humanoid 3D Models` to
  `Humanoid 3D Models for IVH SDK`, to read clearly as Didimo's character package for the
  Intelligent Virtual Humans SDK rather than a standalone SDK of its own.
- `README.md` rewritten and trimmed to an introduction, package description, use case character
  overview and links; the prefab-creation walkthrough and cull mask reference that used to live
  there now have their own documentation pages (see Added).
- Render pipeline assets moved from `Runtime/Scenes` to `Runtime/Settings` (`Mobile_High.asset`, `Mobile_High_Renderer.asset`, `UniversalRenderPipelineGlobalSettings.asset`).
- `Description` now lives in the `Didimo.IVH.Humanoids` namespace instead of the global namespace, where it could collide with consumer code. Root namespaces are declared on the runtime and editor assembly definitions.
- Renamed `Industries Employe` to `Industries Employee` throughout: the prefab, the model folder and its texture set. Asset GUIDs are unchanged, so existing references still resolve.
- Renamed the sample's `New Material` to `Background Material`.
- The six nurse prefabs were renamed to match their model folders and metadata: `Nurse Female African` is now `Nurse African Female`, and likewise for the other five. Asset GUIDs are unchanged, so existing references still resolve.
- Character metadata is now consistent: each character's `description.txt` matches the `Description` component on its prefab, and the prefab file name, root GameObject and `Name:` field all agree.
- `Blendshapes Collection` standardized to `ARKit, OCVR, Visemes` for every character's `description.txt` and prefab `Description` component, including Business Male (previously `Reallusion, OCVR`).
- Business Female and Business Male hair and scalp materials now use `Universal Render Pipeline/Lit`, wired to the replacement hair textures and matching the setup already used by the other ten characters.

### Fixed
- Removed a missing custom reflection cubemap reference from the example scene's render settings.
- Cleared a missing override material reference on the `FullscreenMask` renderer feature.
- Cleared stale texture references on 24 character materials: `_OcclusionMap` on every eye material and on the Business Male hair and scalp, and `_BumpMap` on the Business Male and Business Female mouth materials. The referenced textures were removed in earlier versions but the material references were left behind.
- Removed an orphaned `Editor/Scripts.meta` that had no matching folder.

## [3.0.0] - skipped

Never tagged or released. 3.0.0 was an internal build used for testing only; the version number
was skipped, and the next public release is 3.1.0.

## [2.1.0] - 2026-06-12
- Updated models and textures for all consortium characters and Use Cases Business_Male and Business_Female
- Fixed bug where shader was producing artifacts. 
- Tweaked shader values for better looking characters.
- Added new hair shader to use with some hair assets.

## [2.0.11] - 2026-01-09
- There was a mistake in the previous version README where the video tutorial was removed and a path was missing.
- Along with the locations where cull masks already were (inside each models' folder, together with the rest of the textures that model is using), we included a folder with all the cull masks for ease of use. The README will include a path to this folder. 

## [2.0.10] - 2026-01-09
- Update README file with information regarding cull masks.

## [2.0.9] - 2025-12-05
- Added new cull masks for head and body to every model in the package.

## [2.0.8] - 2025-11-24
- Bugfixes: 
    - Frank's model import settings were not configured correctly and thus missing an Avatar.
    - The Business Female Prefab was referencing a wrong shader on the body material. The reference was updated.

## [2.0.7] - 2025-11-10
- Updated models with hairs and new outfits for Presence Team Characters: Ke Li, Frank Steinicke, Tamas Losonczi and Sergi Fernandez
- Introduces a new shader for the characters' body material that supports the use of culling masks
- Added new culling masks for all Presence Team Characters

## [2.0.6] - 2025-05-26
- Renamed objects that weren't named correctly.
- Place animator component on the correct gameobject.
- Added missing Use Case characters back, also with the new blendshape update.

## [2.0.5] - 2025-05-23
- Updated all the Presence Team characters

## [2.0.4] - 2025-04-24
- Added first version of the use cases characters.

## [2.0.3] - 2025-01-10
- Added animator to the Prefabs that were missing it

## [2.0.2] - 2024-12-18
- Update character models' mouth mesh
- Swapped all URP/Complex Lit shaders for URP/Lit shaders

## [2.0.1] - 2024-12-12
- Update character models
- Update textures
- Update shaders
- New scene setup
- Update render pipeline
- Added a new environment 

## [2.0.0] - 2024-11-25
- Corrections to Models
- Addition of the remaining characters
- Texture Corrections
- Descriptions update
- Setup Humanoid Animation Type

## [1.0.0] - 2024-11-15
- New Set of characters

## [0.0.0] - 2024-08-02
- Set up package structure