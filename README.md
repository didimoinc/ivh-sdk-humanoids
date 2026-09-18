# Humanoid 3D Models for IVH SDK

## Introduction

This package is Didimo's contribution to the **Intelligent Virtual Humans SDK (IVH SDK)**, the
character and avatar infrastructure developed for the [PRESENCE project](https://presence-xr.eu/)
under WP4. It provides the Unity humanoid character prefabs, one per PRESENCE use case, along
with the shaders, cull masks and render pipeline settings needed to render them.

It is designed to sit alongside the SDK's other components in the same way Didimo characters sit
alongside UHH's own toolkit: [uhhhci/intelligent-virtual-agent-sdk](https://github.com/uhhhci/intelligent-virtual-agent-sdk),
the Intelligent Virtual Agent SDK that adds conversation, behaviour and AI on top of a character.
This package supplies the character itself; that SDK (or your own application logic) is what
brings it to life.

More on the IVH SDK and the rest of the PRESENCE developer ecosystem is on the
[PRESENCE Developer Hub](https://presence-xr.eu/developers-hub/). This package lives at
<https://github.com/didimoinc/ivh-sdk-humanoids>.

## Package description

### Unity package and dependencies

| | |
|---|---|
| Package ID | `co.didimo.ivh.humanoids` |
| Unity | 2022.3 or newer |
| Render pipeline | Universal Render Pipeline 14.0.12 (resolved automatically as a dependency) |
| Git LFS | Required, every mesh and texture is stored in LFS |

Full installation and render pipeline setup steps are in the
[documentation index](Documentation~/index.md).

### Use case characters

Each PRESENCE use case has its own character prefab or set of prefabs, built from Didimo's
Character Generation Service:

| Use case | Characters |
|---|---|
| UC1.1 Professional Meeting | Business Female, Business Male |
| UC1.2 Manufacturing | Industries Employee, Industries Manager |
| UC2.1 Health | Nurse African Female, Nurse Caucasian Female, Nurse East Asian Female, Nurse Caucasian Male, Nurse Hispanic Male, Nurse West Asian Male |
| UC2.2 Cultural Heritage | Tourguide, Tourist |

See [Package Contents](Documentation~/PackageContents.md) for demographic detail on every
character, and how to request a new one.

## Documentation

| Page | Covers |
|---|---|
| [Documentation index](Documentation~/index.md) | Requirements, installation, render pipeline setup, package contents |
| [Creating a Prefab from a Generated Character](Documentation~/Prefabs.md) | Turning a Character Generation Service FBX into a prefab |
| [Cull Masks](Documentation~/CullMasks.md) | Which cull mask combination to use for each outfit |
| [First-Person VR Cameras](Documentation~/FirstPersonVRCameras.md) | Hiding a character's own head meshes from its first-person camera |
| [Animation Retargeting](Documentation~/AnimationRetargeting.md) | Retargeting Reallusion/Mixamo animations to PRESENCE characters |
| [Adding Custom Deformables, Outfits and Grooms](Documentation~/AddingCustomDeformables.md) | Fitting a custom deformable hair, groom or outfit to a Didimo avatar |
| [Renaming Blend Shape Targets: Rocketbox to Reallusion](Documentation~/BlendshapeRenaming.md) | Making Rocketbox facial blend shapes compatible with Reallusion-driven animation |
| [Package Contents](Documentation~/PackageContents.md) | Every use case character shipped in this package |
| [FAQ and Current Limitations](Documentation~/FAQ.md) | Runtime loading, Unity Humanoid compatibility, mesh and facial blend shape specifications, adding garments, where to ask for help |

## Requesting a new character

To have a new use case character generated, contact Didimo's Character Generation Service at
<presence@didimo.co>. Once the FBX package arrives, follow
[Creating a Prefab from a Generated Character](Documentation~/Prefabs.md) to bring it into a
Unity project.

## License

See [LICENSE.md](LICENSE.md) for the terms covering this package and the character assets it
contains, and [Third Party Notices.md](Third%20Party%20Notices.md) for third-party content. See
Didimo's privacy policy at <https://privacy.didimo.co/>.

## Funded by the European Union

This work has received funding from the European Union's Horizon Europe research and innovation
programme under grant agreement No 101135025, PRESENCE project.

Views and opinions expressed are however those of the author(s) only and do not necessarily
reflect those of the European Union. Neither the European Union nor the granting authority can be
held responsible for them.
