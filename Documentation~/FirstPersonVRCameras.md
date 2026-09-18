# First-Person VR Cameras: Hiding Head Meshes

In VR, a character's own head, eyes, eyelashes and mouth meshes sit inside its first-person
camera and block the view unless they are hidden from that camera specifically, while staying
fully visible to every other camera (third-person, spectator, mirrors, remote players).

Before | After
:---: | :---:
![Facial meshes blocking the first-person view](Images/FirstPersonVRCameras/01-before-first-person-view.png) | ![Facial meshes hidden from the first-person view](Images/FirstPersonVRCameras/02-after-first-person-view.png)

This package ships three proof-of-concept techniques for doing that as the
**Per-Camera Render POCs** sample (`Samples~/PerCameraRenderPOCs`), importable from
**Window > Package Manager > this package > Samples**. All three target the same five facial
material slots (head, left eye, right eye, eyelashes, mouth) and are scoped per camera, so they
do not change how the avatar looks to anyone else. Depending on your use case, you may need to
hide additional materials.

These are proof-of-concept scenes built with the characters in this package, meant to demonstrate
the approach rather than to be dropped into production directly. They have not been validated
against every project setup, so treat them as a starting point to adapt to your own scene and
constraints.

## Method 1: Duplicate renderer with camera layers

At runtime, the script creates two visual versions of the avatar: the original
`SkinnedMeshRenderer` (all original materials, full character, on the avatar's `ThirdPerson`
layer), and a cloned `SkinnedMeshRenderer` (same mesh, skeleton and bones, but with the head,
eyes, eyelashes and mouth materials replaced by a `NoRender` shader that discards pixels and
writes neither colour nor depth) on a `FirstPerson` layer. Cameras then use culling masks to pick
which renderer they see.

This works best in a multiplayer setting: a real client typically needs only one pair of layers,
since only the locally owned avatar needs the first-person setup. The sample sets up two
characters with their own first-person views at once, which is why it needs four layers (two per
character) instead of two.

**Advantages**

- Camera visibility is explicit and deterministic.
- Multiple cameras can render simultaneously without touching shared state.
- Works naturally with third-person spectator cameras and mirrors.
- Doesn't depend on camera rendering order.

**Disadvantages**

- Mutates every camera's culling mask at runtime.
- Creates an additional `SkinnedMeshRenderer`, so the mesh may be skinned and processed twice.
- Uses more CPU, GPU and memory than a single renderer.
- Unity has a hard limit of 32 layers.

### Method 1 setup

1. Create four Unity layers and leave them unassigned; the script assigns them at runtime:
   `AvatarA_ThirdPerson`, `AvatarA_FirstPerson`, `AvatarB_ThirdPerson`, `AvatarB_FirstPerson`
   (`Edit > Project Settings > Tags and Layers`).

   ![Unity layers used by the sample's two characters](Images/FirstPersonVRCameras/03-method1-unity-layers.png)

   This four-layer setup is only what the two-character demo needs. The technique itself only
   needs one `ThirdPerson`/`FirstPerson` pair per locally owned avatar; remote avatars in a
   multiplayer scene stay as normal third-person avatars and don't need a pair of their own.

2. On the VR camera (the one that sits inside the character's head), add the
   `DoubleRenderVisibility` script and assign the avatar root transform, the `NoRender` material
   and the source `SkinnedMeshRenderer`. Make sure the `Third Person Layer Name` and
   `First Person Layer Name` fields match the layers created in step 1, and that the hidden
   material names match your character's material setup.

   ![DoubleRenderVisibility component on the VR camera](Images/FirstPersonVRCameras/04-method1-component.png)

3. Enter Play mode.

## Method 2: Per-camera material swapping

Keeps only the original `SkinnedMeshRenderer`. Just before the owning VR camera renders, the
script stores the original material array, swaps the five facial material slots for `NoRender`,
lets the camera render, then restores the original materials immediately after.

**Advantages**

- No duplicate skinning, no special Unity layers, smaller runtime hierarchy.
- Lightweight and scales to many locally simulated owners.

**Disadvantages**

- Temporarily changes a renderer's global `sharedMaterials`.
- Depends on render-pipeline begin/end camera callbacks, so it is more sensitive to camera
  ordering, custom render passes and unusual rendering setups.
- Less comfortable for complex production setups involving XR multipass, camera stacking,
  reflections, recording cameras or third-party render features.

### Method 2 setup

1. On the VR camera (the one that sits inside the character's head), add the
   `PerCameraMaterialSwapVisibility` script and assign the avatar root transform and the
   `NoRender` material. Make sure the source renderer name and the hidden material names match
   your character's setup.

   ![PerCameraMaterialSwapVisibility component on the VR camera](Images/FirstPersonVRCameras/05-method2-component.png)

2. Enter Play mode.

## Method 3: Per-camera submesh render feature

Keeps only the original `SkinnedMeshRenderer` and never touches the material array. Just before
the owning VR camera renders, the script sets `forceRenderingOff = true` on the avatar's combined
body renderer, so URP's normal passes skip the entire avatar for that camera. A URP
`ScriptableRendererFeature` then detects that the camera carries the first-person component and
manually redraws only the non-facial (body) submesh. `forceRenderingOff` is restored right after.
The face is never issued as a draw call, rather than discarded by a shader.

This method requires one project-level change: the `ScriptableRendererFeature` must be registered
on the URP Renderer asset (URP 14 has no script-only pass injection).

**Advantages**

- No duplicate skinning and no clone; never mutates `sharedMaterials`.
- Doesn't consume Unity layers; facial submeshes are truly not drawn.
- Third-person, spectator and mirror cameras render the avatar completely untouched.

**Disadvantages**

- `forceRenderingOff` also removes the avatar from that camera's shadow-caster and depth-prepass
  culling, so the manually drawn body will not self-shadow or feed depth-prepass effects unless
  the feature is extended to emit those passes.
- XR single-pass stereo correctness of the manual draw needs on-device verification.
- If the feature isn't on the active renderer, the whole avatar disappears in first-person view.

### Method 3 setup

1. On the VR camera (the one that sits inside the character's head), add the
   `PerCameraSubmeshVisibility` script and assign the avatar root transform. Make sure the source
   renderer name and the hidden material names match your character's setup.

   ![PerCameraSubmeshVisibility component on the VR camera](Images/FirstPersonVRCameras/06-method3-component.png)

2. Enter Play mode.

## Summary

Method 1 is the most robust and deterministic, at the highest runtime cost and subject to Unity's
32-layer ceiling. Method 2 is the lightest-weight and simplest to set up, but relies on precise
camera-callback timing and temporarily mutates shared materials. Method 3 gives the cleanest
result, facial submeshes are never drawn and materials are never touched, at the cost of extra
complexity around shadows and depth-prepass and a required renderer asset change.

## Trying the sample

1. Import **Per-Camera Render POCs** from the Package Manager.
2. For Method 1 only, add the four layers described under Method 1 setup above.
3. For Method 3 only, register `FirstPersonSubmeshRenderFeature` on your URP Renderer asset
   (`Add Renderer Feature > First Person Submesh Render Feature`).
4. Open `Example1`, `Example2` or `Example3` and enter Play mode. Each scene has two characters
   facing each other, with a screen behind each one showing its own first-person view. Before
   entering Play mode, each character's own facial meshes are visible on its screen; after
   entering Play mode, they disappear from that character's own view while the other character
   still renders fully.

The sample's characters are prefab variants of this package's own `Business Male` and
`Business Female` prefabs, and its materials and shaders come from the package itself, so it stays
in sync with the models it demonstrates rather than carrying duplicate copies.
