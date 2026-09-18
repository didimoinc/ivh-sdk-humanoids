# FAQ and Current Limitations

## Can characters be generated or swapped at runtime?

No. Characters from this package cannot be loaded or generated at runtime. Turning a Character
Generation Service output into a usable prefab is an editor-time process, see
[Creating a Prefab from a Generated Character](Prefabs.md), and the result is a regular Unity
prefab baked into your project, not an asset streamed or assembled while the application is
running.

## Are the characters compatible with Unity?

Yes. The body rig is compatible with the Unity Humanoid rig. This is what enables retargeting
standard Humanoid animations, including Mixamo and Reallusion clips (see
[Animation Retargeting](AnimationRetargeting.md)). However, it also means the characters inherit
Unity Humanoid's own constraints, for example a single root bone and no support for extra bones
outside the standard Humanoid bone set.

## What are the character mesh and material specifications?

Each character is a single mesh with sub-meshes (sub-materials), rather than separate meshes per
body part (head, body, eyes, etc.). Garments and hair (attachables) are separate meshes, each with
its own material. This is why the cull mask system exists: to hide part of the underlying body
when an outfit covers it. It also means techniques that expect independently toggleable meshes
(see [First-Person VR Cameras](FirstPersonVRCameras.md)) have to work at the material or sub-mesh
level instead.

## What are the facial animation specifications?

Facial animation works with blend shapes. The facial rig and blend shapes are set up for three
collections: ARKit, OCVR (Oculus's VR facial tracking), and Visemes (lip-sync).

## Can I change the names of the channels for facial animation blend shapes?

Yes. The FBX can be modified to rename or adapt the channels to other configurations (see
[Renaming Blend Shape Targets](BlendshapeRenaming.md)).

## Are the characters compatible with Mixamo-produced animations?

Not directly, but an animation retargeting workflow through a DCC application can be used (see
[Animation Retargeting](AnimationRetargeting.md)).

## Can I add new garments or hair to the characters?

Yes, this can be done manually. Or, as an example, there is a guideline for adapting garments from
Reallusion (see
[Adding Custom Deformables, Outfits and Grooms](AddingCustomDeformables.md)).

## Where do I ask for a new character?

Contact Didimo's Character Generation Service at <presence@didimo.co>. See
[Package Contents](PackageContents.md) for what already exists and
[Creating a Prefab from a Generated Character](Prefabs.md) for the import steps.

## Where do I report a bug or ask something not covered here?

Reach out to the package maintainers at <presence@didimo.co>.
