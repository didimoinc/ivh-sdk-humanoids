# Animation Retargeting: Reallusion / Mixamo to PRESENCE

This page explains how to make Reallusion and Mixamo animations compatible with the PRESENCE
characters in this package, avoiding the geometry collapse issues that can happen when an
animation authored for a different skeleton is applied directly.

Templates and the characterization profile referenced below are kept in this Drive folder:
<https://drive.google.com/drive/folders/1XkTBCtbNK7BHXEBiWztdXUTnOwLQrkOU>

**Software dependencies:** Reallusion Character Creator 4, and the Mixamo animation service.

## Step by step: retargeting a Reallusion animation to a PRESENCE character

### 1. Open Character Creator 4 and create a new project

![File menu, New Project](Images/Shared/cc4-new-project.png)

Create a new project in a folder.

### 2. Import the PRESENCE template

![Import menu](Images/AnimationRetargeting/02-import-menu.png)
![Import FBX, Character](Images/AnimationRetargeting/03-import-fbx-character.png)

Import the template FBX file from the Templates folder linked above. Use the male template for
male retargets and the female template for female retargets. Select **Character** as the type and
**Humanoid (Non-Standard)**.

![Template imported in T-pose](Images/AnimationRetargeting/04-imported-template-tpose.png)

The character appears without texture, which does not matter for retargeting.

### 3. Characterize the template

Characterization maps the bones between the two skeletal structures.

![Characterization tab, loading the custom profile](Images/AnimationRetargeting/05-characterization-custom-profile.png)

Go to the **Characterization** tab and load the `RealIussion2Presence.3dxProfile` profile from
the Profile folder linked above.

![Apply Profile dialog](Images/AnimationRetargeting/06-apply-profile-dialog.png)

Keep **T-Pose** and **Bone Mapping** active.

![HumanIK active after characterization](Images/AnimationRetargeting/07-humanik-active.png)

Activate **Human IK** on the same tab.

![Save Project](Images/AnimationRetargeting/08-save-project.png)

We recommend saving the Character Creator project at this point, since characterization only
needs to be done once per template.

### 4. Add a Reallusion animation

![Picking an animation from the content browser](Images/AnimationRetargeting/09-pick-animation.png)

In the left panel, find the animation you want and double-click it. The retargeting happens
automatically.

### 5. Export the animation

![Export menu](Images/AnimationRetargeting/10-export-menu.png)
![Export FBX dialog](Images/AnimationRetargeting/11-export-fbx-dialog.png)

Go to **Export > FBX > Selected** and configure the export options as shown above: target tool
preset `Unity 3D`, FBX option `Motion`, frame rate matching your source clip, and **Current
Animation > All**.

### 6. Import the animation in Unity

![Imported motion clip in a Unity project](Images/AnimationRetargeting/12-unity-import-result.png)

Import only the animation, not the full template, into your Unity project alongside this
package's character prefabs.

## Credit

This workflow was documented as part of Didimo's WP4 reference material.
