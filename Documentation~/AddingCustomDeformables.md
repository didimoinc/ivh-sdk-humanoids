# Adding Custom Deformables, Outfits and Grooms

This page explains how to fit a custom hair, groom or clothing asset (a deformable) onto a Didimo
avatar so it deforms correctly with the character's animation.

The full reference manual and source files (the conforming blend shape geometry and a sample
Didimo avatar) are kept in this Drive folder:
<https://drive.google.com/drive/folders/1FA2QGsIfS0s7a8L8yoYLNRcPyFD4G8qW>

**Software dependencies:** Reallusion Character Creator 4, Autodesk Maya (2025 to 2027). The same
result can also be reached with other software such as Blender, or directly during character
creation; see the Appendix for fitting assets without Maya.

## Step by step: custom deformables

### 1. Open Character Creator 4 and create a new project

### 2. Load the Kevin character

![Loading the CC4 Kevin base character](Images/CustomDeformables/02-load-kevin-character.jpg)

**Actor > Character > CC4 Kevin**, then double-click the character.

### 3. Add the custom accessory

![Adding a clothing accessory](Images/CustomDeformables/03-add-accessory.jpg)

Drag and drop the asset you want to fit, for example a t-shirt from **Cloth > Shirts**.

### 4. Put the character in T-pose

![Calibration T-Pose](Images/CustomDeformables/04-tpose-calibration.jpg)

**Animation > Pose > Calibration > T-Pose**, then double-click the pose.

### 5. Detach the deformable and export

![Convert to Accessory, Bake Current Shape](Images/CustomDeformables/05-convert-to-accessory.jpg)

1. Double-click the deformable (cloth or hair) and convert it to an accessory, baking the current
   shape.

![Detach](Images/CustomDeformables/06-detach.jpg)

2. Click **Detach**.

![The isolated deformable after deleting the body](Images/CustomDeformables/07-isolated-deformable.jpg)

3. Double-click the body and delete it. The isolated deformable can now be exported.

![Export > FBX > Selected](Images/CustomDeformables/08-export-fbx-menu.jpg)
![FBX export texture settings](Images/CustomDeformables/09-export-fbx-texture-settings.jpg)

4. Export the geometry, unchecking **Embed Textures** and converting the image format to PNG.

![Export folder before cleanup](Images/CustomDeformables/10-export-folder-before.jpg)
![Export folder after cleanup](Images/CustomDeformables/11-export-folder-after.jpg)

5. Clean up the export folder: delete the `.fbm` and `.json` files, and move the textures into
   the same folder as the FBX.

### 6. Clean up the geometry in Maya

![Before repositioning: wrong pose and bone rig still attached](Images/CustomDeformables/12-maya-before-reposition.jpg)

The exported FBX comes in with the wrong pose and a bone rig still attached.

1. Move the time slider to frame 1 to reposition the deformable.
2. Unparent the geometry (`Shift+P`).
3. Delete all animation: **Edit > Delete All by Type > Channels**.
4. Select the geometry and freeze transforms: **Modify > Freeze Transformation**.
5. Delete the bone in the Outliner.
6. Save the file.

![After repositioning and cleanup](Images/CustomDeformables/13-maya-after-reposition.jpg)

### 7. Conform the shape to the Didimo topology

This step moves the deformable from the Kevin topology space into the Didimo topology space,
using the `blendShapeConform` geometry provided in the Drive folder above (available in both Maya
and FBX format).

![Proximity Wrap on the body geometry](Images/CustomDeformables/14-proximity-wrap.png)

1. Import `blendShapeConform` alongside the deformable.
2. Select the deformable geometry and go to **Rigging > Deform > Proximity Wrap** to create a
   geometry wrap.

![Proximity Wrap Attributes: Offset mode, Falloff Scale 10](Images/CustomDeformables/15-proximity-wrap-attributes.png)

3. With the deformable selected, open the Attribute Editor (`Ctrl+A`) and navigate to the
   Proximity Wrap node.
4. Select the body geometry, then **Manage Drivers > Add Selected**. Set **Wrap Mode** to
   `Offset` and **Falloff Scale** to `10`.

![Activating the Didimo blend shape](Images/CustomDeformables/16-didimo-blendshape.png)

5. Activate the `Didimo` blend shape from **Animation Editors > Shape Editor**.
6. Clean up the garment: select its geometry and **Edit > Delete by Type > History** to remove
   the deformers, then delete the body.
7. Re-export the FBX.

### 8. Bind the geometry

![Copy Skin Weights options](Images/CustomDeformables/17-copy-skin-weights.png)

1. Open the avatar (for example `DidimoAvatar.fbx`, provided in the Drive folder above).
2. Import the deformable geometry.
3. Select the body, then the deformable.
4. **Rigging > Skin > Copy Skin Weights**, with **Surface Association** set to `Closest point on
   surface` and **Influence Association 1** set to `One to one`.

The avatar with the new deformable can now be imported into Unity as usual.

## Appendix: fitting assets with Character Creator 4 only

In Character Creator 4, attaching clothes without Maya involves importing the model as an
accessory and then converting it into cloth by transferring the body's skin weights.

1. **Import the clothing as an accessory:** go to **Create > Accessory** and import an OBJ or FBX
   file. Use the transform tools to scale and position it against the character's body, and use
   Edit Mesh or Pose Editing to fix any poke-through where the body sticks out of the clothes.
2. **Convert to cloth:** transfer the body's skin weights to the clothing so it moves naturally
   with the character's animation.
3. **Refine the fit** once the cloth is skinned, adjusting for any remaining poke-through.

Rigid items, such as accessories that shouldn't deform (glasses, jewelry), can usually be parented
directly to a bone instead of going through this skin-weighting process.

## Credit

This workflow was documented as part of Didimo's WP4 reference material.
