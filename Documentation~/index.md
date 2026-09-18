# Humanoid 3D Models for IVH SDK

Ready-to-use humanoid character prefabs for the PRESENCE use cases, together with the URP
shaders, culling masks and render pipeline settings needed to render them. This package is
Didimo's contribution to the Intelligent Virtual Humans SDK (IVH SDK) for PRESENCE WP4.

## Documentation pages

| Page | Covers |
|---|---|
| [Creating a Prefab from a Generated Character](Prefabs.md) | Turning a Character Generation Service FBX into a prefab |
| [Cull Masks](CullMasks.md) | Which cull mask combination to use for each outfit |
| [First-Person VR Cameras](FirstPersonVRCameras.md) | Hiding a character's own head meshes from its first-person camera |
| [Animation Retargeting](AnimationRetargeting.md) | Retargeting Reallusion/Mixamo animations to PRESENCE characters |
| [Adding Custom Deformables, Outfits and Grooms](AddingCustomDeformables.md) | Fitting a custom deformable hair, groom or outfit to a Didimo avatar |
| [Renaming Blend Shape Targets: Rocketbox to Reallusion](BlendshapeRenaming.md) | Making Rocketbox facial blend shapes compatible with Reallusion-driven animation |
| [Package Contents](PackageContents.md) | Every use case character shipped in this package |
| [FAQ and Current Limitations](FAQ.md) | Runtime loading, Unity Humanoid compatibility, mesh and facial blend shape specifications, adding garments, where to ask for help |

## Requirements

| | |
|---|---|
| Unity | 2022.3 or newer |
| Render pipeline | Universal Render Pipeline 14.0.12 (resolved automatically as a package dependency) |
| Git LFS | **Required.** See below. |

### Git LFS is mandatory

Every mesh and texture in this package is stored in Git LFS. The Unity Package Manager installs
git-URL packages with a plain `git clone`, so if `git-lfs` is not installed and on your `PATH`
you will receive pointer files instead of assets and the package will not work.

Install it from [git-lfs.com](https://git-lfs.com) and run `git lfs install` once before adding
the package.

## Installation

Add the package through **Window > Package Manager > + > Add package from git URL**, or add it
directly to `Packages/manifest.json`:

```json
"co.didimo.ivh.humanoids": "https://github.com/didimoinc/ivh-sdk-humanoids.git"
```

## Render pipeline setup

The characters are authored against a specific URP configuration that ships with the package. To
reproduce the intended look:

1. Open **Edit > Project Settings > Graphics**.
2. Set **Scriptable Render Pipeline Settings** to `Runtime/Settings/Mobile_High.asset`.
3. Open **Edit > Project Settings > Quality** and set the same asset on the quality level(s) you
   ship with.
4. Make sure the project colour space is **Linear** (**Project Settings > Player > Other
   Settings > Color Space**).

Without step 2 the project falls back to the Built-in Render Pipeline and every character
renders magenta.

## Importing the example scene

The package ships a sample that demonstrates every prefab in a lit environment:

**Window > Package Manager >** select this package **> Samples > Example Scene > Import**

The sample is copied to `Assets/Samples/Humanoid 3D Models for IVH SDK/<version>/Example Scene/`.
Open `ExampleScene.unity` from there. It carries its own baked lighting, reflection probes,
environment mesh and skybox, so it should look identical to the reference once the render
pipeline is configured as above.

To include the scene in a build, add it to **File > Build Settings > Scenes In Build** — samples
are not added automatically.

For hiding a character's own head meshes from its first-person VR camera, see the
**Per-Camera Render POCs** sample and [First-Person VR Cameras](FirstPersonVRCameras.md).

## Contents

| Path | Contents |
|---|---|
| `Runtime/Prefabs` | The character prefabs |
| `Runtime/Models` | Meshes, textures and materials, grouped by use case |
| `Runtime/Shader` | `URP_LitWithCulls` and `S_Clothing` |
| `Runtime/Textures/CullMasks` | Culling masks for all characters, collected in one place |
| `Runtime/Settings` | URP pipeline asset, renderer and global settings |
| `Runtime/Scripts` | `Description`, which carries per-character metadata |
| `Samples~/ExampleScene` | The importable example scene |
| `Samples~/PerCameraRenderPOCs` | The first-person camera reference scenes |

See [Package Contents](PackageContents.md) for the full list of use case characters.

## Shaders

| Shader | Used for |
|---|---|
| `URP_LitWithCulls` | `ddmo_body_MAT` and `ddmo_head_MAT`, which need culling-mask support |
| `S_Clothing` | Deformable clothing materials |
| `Universal Render Pipeline/Lit` | Hair, scalp, and all other `ddmo_*` materials |

Import settings for materials created from a fresh FBX are documented in
[Creating a Prefab from a Generated Character](Prefabs.md).

## Third-party content

See `Third Party Notices.md` at the package root.

## License

See `LICENSE.md` at the package root, and Didimo's privacy policy at
<https://privacy.didimo.co/>.
