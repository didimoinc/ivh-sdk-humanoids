# Creating a Prefab from a Generated Character

This page walks through turning an FBX package from Didimo's Character Generation Service into a
Unity prefab that matches the setup used by every character already in this package.

## 1. Import the FBX

After downloading the character from the Character Generation Service, add the folder with the
FBX and textures to your Unity project, then click the FBX to open the FBX Import Settings and go
to the **Rig** tab.

## 2. Set the animation type

Set **Animation Type** to `Humanoid` and **Avatar Definition** to `Create From This Model`.

## 3. Configure the avatar

Click **Configure** to open the Avatar Configuration page, choose the **Muscles & Settings** tab
and make sure **Translation DoF** is set to `True`. Click **Done**.

Video tutorial: <https://drive.google.com/file/d/1DV-kBs0WvMz4uS196BNGAtu6FRjx3Ifx/view?resourcekey>

## 4. Duplicate the materials

Expand the FBX, select all the materials and duplicate them. We recommend saving the duplicates
inside the character's own folder, so the prefab never points back at the original FBX's
materials.

## 5. Assign the correct shaders

All `ddmo_[Body Part]_MAT` materials should use `Universal Render Pipeline/Lit`, except for
`ddmo_body_MAT` and `ddmo_head_MAT`, which need the package's own `URP_LitWithCulls` shader for
cull mask support. Deformable clothing materials should use the package's `S_Clothing` shader.
Hair and scalp materials use `Universal Render Pipeline/Lit`.

Use these settings for the `Universal Render Pipeline/Lit` materials (a flag not mentioned below
should stay OFF):

**Eyelashes**

- Workflow Mode: Specular
- Surface Type: Transparent
- Blending Mode: Alpha
- Render Face: Front
- Alpha Clipping: ON, Threshold 0.25
- Receive Shadows: ON

**Eyes and mouth**

- Workflow Mode: Specular
- Surface Type: Opaque
- Render Face: Front
- Receive Shadows: ON

**Scalp**

- Workflow Mode: Metallic
- Surface Type: Transparent
- Blending Mode: Alpha
- Render Face: Both
- Alpha Clipping: ON, Threshold 0.05
- Receive Shadows: ON

## 6. Assign the textures

Unity usually fills in albedo and normal maps automatically from the FBX, but you will need to
drag in Specular and Occlusion maps for skin materials, and Mask Maps for deformables. You also
need to assign the cull masks to the body material's `MaskTexture` slot; see
[Cull Masks](CullMasks.md) for which mask goes with which outfit.

## 7. Create the prefab

Right-click inside the Project tab and choose **Create > Prefab**.

## 8. Add the Animator

Open the new prefab to enter the Prefab Editor, add an **Animator** component, and drag the
Avatar asset generated under the FBX into the component's **Avatar** slot.

## 9. Add the FBX to the prefab

Drag the FBX inside the prefab. You can optionally unpack the FBX to break the connection to the
original file, or leave it connected so that future changes to the FBX are reflected in the
prefab.

## 10. Swap in the duplicated materials

Replace every material used by the Skinned Mesh Renderers with the duplicated materials you set
up in step 4.

## Next steps

- [Cull Masks](CullMasks.md): which cull mask combination to use for each outfit.
- [Package Contents](PackageContents.md): the use case characters already shipped this way.
