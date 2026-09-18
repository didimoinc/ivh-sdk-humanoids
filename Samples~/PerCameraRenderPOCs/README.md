# Per-Camera Render POCs

Reference material, not a runnable demo.

Three scenes solving the **same** problem three different ways: hide a humanoid's head, eyes,
eyelashes and mouth from that avatar's own first-person camera, while keeping them fully visible
to every other camera in the scene. All three produce the same visual result — they differ only in
technique, in cost, and in how well they survive a real production setup.

Each scene is deliberately minimal: two avatars, a TV showing what a first-person camera sees, and
a shared environment. Open one, read its script, and pick the approach that fits your project.

## Before you start

Two setup steps live in **Project Settings**, which a Unity package cannot ship for you.

### Layers — required by Method 1 only

Add these four layers under **Edit > Project Settings > Tags and Layers**. The indices below are
the ones used in the source project; any free index works, because the scripts resolve layers **by
name**:

| Index | Layer name | Used by |
|---|---|---|
| 28 | `AvatarA_ThirdPerson` | Business Male Variant |
| 29 | `AvatarA_FirstPerson` | Business Male Variant |
| 30 | `AvatarB_ThirdPerson` | Business Female Variant |
| 31 | `AvatarB_FirstPerson` | Business Female Variant |

If a layer is missing, `DoubleRenderVisibility` logs *"First-person avatar layers are missing"* and
disables itself.

Four layers are needed only because this demo runs **two** local first-person avatars side by side
so you can see both perspectives at once. A real multiplayer client needs just **one** pair: the
locally-owned avatar gets the first-person handling, and every remote avatar stays an ordinary
third-person avatar.

**Methods 2 and 3 need no custom layers.**

### Renderer feature — required by Method 3 only

`FirstPersonSubmeshRenderFeature` is a `ScriptableRendererFeature`, and URP 14 has no script-only
pass injection — Unity only runs it if it is registered on the renderer your camera actually uses.

Select the URP Renderer asset in use (for this package's own configuration that is
`Runtime/Settings/Mobile_High_Renderer.asset`) and choose **Add Renderer Feature > First Person
Submesh Render Feature**.

It is not pre-registered on any renderer asset, deliberately, so that it cannot affect the other
two scenes or the rest of the package. If it is missing when you open `Example3`, the whole avatar
disappears from the first-person view — body included.

---

## Method 1 — Duplicate renderer with camera layers (`Example1`)

At runtime the script creates two visual versions of the avatar:

1. **The original `SkinnedMeshRenderer`** — all original materials, the complete character, on the
   avatar's `ThirdPerson` layer.
2. **A cloned `SkinnedMeshRenderer`** — same mesh, skeleton and bones, but the head, eyes,
   eyelashes and mouth materials are replaced with `NoRender`. It lives on the avatar's
   `FirstPerson` layer.

The `NoRender` shader discards its pixels and writes neither colour nor depth. Cameras then use
culling masks to choose which renderer they see.

This method works best in a multiplayer setting.

**Advantages**

- Camera visibility is explicit and deterministic.
- Multiple cameras can render simultaneously without changing shared state.
- Works naturally with third-person spectator cameras, such as mirrors.
- The Culling Mask shows exactly what a camera sees.
- Does not depend on camera rendering order.
- In real multiplayer, only the locally-owned avatar needs the special setup.

**Disadvantages**

- Mutates every camera's culling mask at runtime.
- Creates an additional `SkinnedMeshRenderer`.
- The mesh may be skinned and processed twice.
- Uses more CPU, GPU and memory than a single renderer.
- Blendshapes and changing renderer settings need synchronisation.
- Hidden submeshes still submit inexpensive `NoRender` draw work — they are not truly removed.
- Unity has a hard limit of 32 layers.

---

## Method 2 — Per-camera material swapping (`Example2`)

Keeps only the original `SkinnedMeshRenderer`. When the owning VR camera is about to render:

1. The script stores (or reuses) the original material array.
2. It replaces the five facial material slots with `NoRender`.
3. The VR camera renders the avatar without those facial submeshes.
4. Immediately after that camera finishes, the original materials are restored.

**Frame sequence**

```
VR Camera A begins
    hide Avatar A facial materials
    render Camera A
    restore Avatar A materials
VR Camera B begins
    hide Avatar B facial materials
    render Camera B
    restore Avatar B materials
Main Camera begins
    both avatars already have their original materials
    render both complete characters
```

Each component reacts only to its owning camera, so Camera A never swaps Avatar B's materials and
Avatar B looks normal from Camera A's perspective.

**Advantages**

- No duplicate skinning.
- No special Unity layers.
- Smaller runtime hierarchy.
- Scales to many locally-simulated owners without consuming a layer pair each (not relevant in a
  multiplayer setup).
- Lightweight.

**Disadvantages**

- Temporarily changes a renderer's global `sharedMaterials`.
- Depends on render-pipeline begin/end camera callbacks.
- More sensitive to camera ordering, custom render passes and unusual rendering systems.
- Materials always appear restored when inspected outside the brief render callback, which makes it
  harder to debug visually in the Inspector.
- Hidden submeshes still submit inexpensive `NoRender` draw work — they are not truly removed.
- Reassigning material arrays every camera render can interfere with batching or add CPU overhead.
- A failed or unpaired render callback could briefly leave the wrong material state, though the
  script contains cleanup safeguards.
- Less comfortable for complex production setups involving XR multipass, camera stacking,
  reflections, recording cameras or third-party render features.

---

## Method 3 — Per-camera submesh render feature (`Example3`)

Keeps only the original `SkinnedMeshRenderer` and never touches the material array. When the owning
VR camera is about to render:

1. The script sets `forceRenderingOff = true` on the avatar's combined `CC_Base_Body` renderer, so
   URP's normal passes skip the entire avatar — face and body — for that camera.
2. A URP `ScriptableRendererFeature` detects that the camera being rendered carries the first-person
   component, and manually re-draws only the non-facial submeshes (the body slot).
3. Immediately after that camera finishes, `forceRenderingOff` is restored to `false`.

The face is never drawn because a draw call for it is never issued — it is omitted, not discarded by
a shader. The body is drawn with its real material and the live skinned pose.

**Frame sequence**

```
VR Camera A begins
    force Avatar A's renderer off (skipped by the normal passes)
    render Camera A (scene + everyone except Avatar A)
    feature re-draws Avatar A's body submesh only
    restore Avatar A's renderer
VR Camera B begins
    same, for Avatar B
Main Camera begins
    neither avatar is forced off; the feature is inert (no first-person component)
    render both complete characters
```

**Advantages**

- No duplicate skinning and no clone.
- Never mutates `sharedMaterials` — the material array is always intact, even mid-render.
- Consumes no Unity layers.
- The facial submeshes are truly not drawn.
- Per-camera draw selection is deterministic by construction: the feature keys off a component only
  the owning camera has, so nothing leaks between cameras.
- Third-person, spectator and mirror cameras render the avatar completely untouched.
- True per-submesh control on a combined mesh.

**Disadvantages**

- Depends on render-pipeline begin/end callbacks (to toggle `forceRenderingOff`) *and* on a custom
  render pass.
- `forceRenderingOff` also removes the avatar from that camera's shadow-caster and depth-prepass
  culling, so the manually-drawn body will not self-shadow or feed depth-prepass effects in the
  first-person view unless the feature is extended to emit ShadowCaster/DepthOnly passes.
- The manual draw replicates only the forward pass.
- Relies on `forceRenderingOff` not skipping skinning. This was mitigated with
  `updateWhenOffscreen = true`, but needs validation, and it interacts with camera render order
  (whether the mesh was skinned yet this frame).
- Like Method 2, suppression uses a brief windowed global flag, so it depends on sequential
  begin/end and is not as bulletproof for genuinely simultaneous or parallel camera rendering as
  Method 1's persistent layers.
- XR single-pass stereo correctness of the manual draw must be verified on-device.
- If the feature is not on the active renderer, the whole avatar disappears in the first-person
  view, body included.

---

## Layout

| Path | Contents |
|---|---|
| `Common/Scene/Environment/` | Ground mesh, skybox and lighting settings shared by all three scenes |
| `Common/Tv.prefab` | The in-scene screen that displays a first-person camera's output |
| `Common/NoRender.shader` / `.mat` | Invisible material used by Methods 1 and 2 |
| `Common/FirstPersonPerspective*.renderTexture` | Targets the VR cameras render into |
| `DoubleRenderPOC/` | Method 1 |
| `PerCameraMaterialSwapPOC/` | Method 2 |
| `SubmeshRenderFeaturePOC/` | Method 3 |

Each POC folder holds its own `Business Male Variant` and `Business Female Variant`. These are
prefab **variants** of the package's own `Runtime/Prefabs/Business Male` and `Business Female`:
they add the VR camera rig and that method's visibility script, and override nothing else but the
name and the spawn position. The characters, their materials and the shaders all come from the
package itself rather than from duplicated copies, so the sample stays small and cannot drift out
of sync with the models it demonstrates.
