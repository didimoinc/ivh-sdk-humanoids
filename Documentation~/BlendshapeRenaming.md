# Renaming Blend Shape Targets: Rocketbox to Reallusion

This page explains how to make the facial animation blend shape targets from Microsoft Rocketbox
avatars compatible with Reallusion-driven animation. The guide was prepared using Maya (2022 to
2026); the same principle can be applied in other DCC applications, such as Blender.

## Step by step

### 1. Download the Rocketbox file

![Rocketbox repository, Male_Adult_01_facial.fbx](Images/BlendshapeRenaming/01-rocketbox-repo.png)

Go to the [Rocketbox repository](https://github.com/microsoft/Microsoft-Rocketbox/tree/master),
select an adult avatar, and download the `_facial` version. This file contains the blend shapes.

### 2. Open the FBX file in Maya and delete the target geometries

![Deleting the hidden target geometries in the Outliner](Images/BlendshapeRenaming/02-delete-target-geometries.png)

Select the grey (hidden) geometries in the Outliner and press Delete.

### 3. Open the renaming Python script

![Script Editor with the renaming script loaded](Images/BlendshapeRenaming/03-script-editor.png)

Open the Script Editor and load `renameRocketBox2Reallusion.py` (see Appendix A below).

### 4. Point the script at the JSON file and execute

In the script, change the `jsonFile` path (line 59) to your local `face_targets.json` file (see
Appendix A), then click Play in the Script Editor. The blend shape targets are renamed.

### 5. Export to FBX

![Export All Options: FBX export](Images/BlendshapeRenaming/04-export-all-options.png)

Go to **File > Export All**, select **FBX export** as the file type.

![Export dialog, FBX 2011 format selected](Images/BlendshapeRenaming/05-export-fbx2011.png)

Under **Advanced Options > FBX File Format**, set **Version** to `FBX 2011`; this maximizes
compatibility with Unity. Click **Export All**. The process is now complete.

## Current limitations

- The Rocketbox blend shapes still carry some of their old targets. A future improvement would be
  to delete those legacy targets.
- These blend shapes cannot be added, since they don't exist in the Rocketbox target list:
  `Brow_Raise_Inner_L`, `Brow_Raise_Inner_R`, `Tongue_Bulge_L`, `Tongue_Bulge_R`.
- This process achieves full compatibility with Oculus VR (OCVR) facial animation, but not with
  the Reallusion shape set (ARKit and FACS): for example, `Brow_Raise` is a single blend shape in
  Rocketbox but two blend shapes in Reallusion, and some secondary shapes are missing.

## Appendix A: code

### `renameRocketBox2Reallusion.py`

```python
from maya import cmds
import json

# UTILS #
def rename_blendshape_target(blendshape_node, old_alias, new_alias):
    '''
    rename_blendshape_target('C_baseShape_BLS', 'C_eyebrowUp_PLY', 'C_eyebrowDown_PLY')

    Parameters
    ----------
    blendshape_node : string
        Name of the blendshape node that contains the blendshape targets
    old_alias : string
        Old blendshape target name
    new_alias : string
        New blendshape target name
    '''

    all_aliases = cmds.aliasAttr(blendshape_node, q=True)
    if not old_alias in all_aliases:
        raise ValueError(
            "BlendShape node '{blendshape_node}' doesn't have an alias '{old_alias}'".format(**locals()))
    old_alias_attr_index = all_aliases.index(old_alias) + 1
    old_alias_attr = all_aliases[old_alias_attr_index]
    cmds.aliasAttr(new_alias, '{blendshape_node}.{old_alias_attr}'.format(**locals()))


# MAIN #
def renameRocketBox2Reallusion(jsonFile, blendshape_node):
    """
    Rename the blendshape targets based on a list of objects and a json

    Parameters
    ----------
    jsonFile : unicode
        Path to the target names json file
    blendshape_node : string, optional
        Name of the blendshape node that contains the blendshape targets
    """

    # Read poses list from the json
    valueData = {}

    with open(jsonFile) as json_file:
        data = json.load(json_file)
        for key, value in data.items():
            if key == "RocketBoxToReallusion":
                valueData.update(value)

    # Rename the blendshape targets
    for key, name in valueData.items():
        try:
            rename_blendshape_target(blendshape_node, name, key)
        except:
            cmds.warning("Blendshape target " + name + " is renamed or does not exist")
            pass


# RUN #
jsonFile = r"C:\Didimo\source\31_PresenceRocketBox\00_script\face_targets.json"
blendshapeNodeName = 'blendShape1'

renameRocketBox2Reallusion(jsonFile, blendshapeNodeName)
```

### `face_targets.json`

```json
{
  "RocketBoxToReallusion": {
    "Brow_Raise_Outer_L": "AK_04_BrowOuterUpLeft",
    "Brow_Raise_Outer_R": "AK_05_BrowOuterUpRight",
    "Brow_Drop_L": "AK_01_BrowDownLeft",
    "Brow_Drop_R": "AK_02_BrowDownRight",
    "Eye_Blink_L": "AK_09_EyeBlinkLeft",
    "Eye_Blink_R": "AK_10_EyeBlinkRight",
    "Eye_Squint_L": "AK_07_CheekSquintLeft",
    "Eye_Squint_R": "AK_08_CheekSquintRight",
    "Eye_Wide_L": "AK_21_EyeWideLeft",
    "Eye_Wide_R": "AK_22_EyeWideRight",
    "Eye_L_Look_L": "AK_15_EyeLookOutLeft",
    "Eye_R_Look_L": "AK_14_EyeLookInRight",
    "Eye_L_Look_R": "AK_13_EyeLookInLeft",
    "Eye_R_Look_R": "AK_16_EyeLookOutRight",
    "Eye_L_Look_Up": "AK_17_EyeLookUpLeft",
    "Eye_R_Look_Up": "AK_18_EyeLookUpRight",
    "Eye_L_Look_Down": "AK_11_EyeLookDownLeft",
    "Eye_R_Look_Down": "AK_12_EyeLookDownRight",
    "Nose_Sneer_L": "AK_50_NoseSneerLeft",
    "Nose_Sneer_R": "AK_51_NoseSneerRight",
    "Cheek_Raise_L": "AU_06_L_CheekRaiser",
    "Cheek_Raise_R": "AU_06_R_CheekRaiser",
    "Cheek_Puff_L": "SR_01_Cheek_Puff_Left",
    "Cheek_Puff_R": "SR_02_Cheek_Puff_Right",
    "Mouth_Smile_L": "AK_44_MouthSmileLeft",
    "Mouth_Smile_R": "AK_45_MouthSmileRight",
    "Mouth_Frown_L": "AK_30_MouthFrownLeft",
    "Mouth_Frown_R": "AK_31_MouthFrownRight",
    "Mouth_Stretch_L": "AK_46_MouthStretchLeft",
    "Mouth_Stretch_R": "AK_47_MouthStretchRight",
    "Mouth_Dimple_L": "AK_28_MouthDimpleLeft",
    "Mouth_Dimple_R": "AK_29_MouthDimpleRight",
    "Mouth_Press_L": "AK_36_MouthPressLeft",
    "Mouth_Press_R": "AK_37_MouthPressRight",
    "Mouth_Pucker": "AK_38_MouthPucker",
    "Mouth_Funnel": "AK_32_MouthFunnel",
    "Mouth_Roll_In_Upper": "AK_41_MouthRollUpper",
    "Mouth_Roll_In_Lower": "AK_40_MouthRollLower",
    "Mouth_L": "AK_33_MouthLeft",
    "Mouth_R": "AK_39_MouthRight",
    "Mouth_Shrug_Upper": "AK_43_MouthShrugUpper",
    "Mouth_Shrug_Lower": "AK_42_MouthShrugLower",
    "Mouth_Up_Upper_L": "AK_48_MouthUpperUpLeft",
    "Mouth_Up_Upper_R": "AK_49_MouthUpperUpRight",
    "Mouth_Down_Lower_L": "AK_34_MouthLowerDownLeft",
    "Mouth_Down_Lower_R": "AK_35_MouthLowerDownRight",
    "Mouth_Close": "AK_27_MouthClose",
    "Jaw_Open": "AK_25_JawOpen",
    "Jaw_Forward": "AK_23_JawForward",
    "Jaw_R": "AK_26_JawRight",
    "Jaw_L": "AK_24_JawLeft",
    "OCVR_U": "AA_VI_14_U",
    "OCVR_sil": "AA_VI_00_Sil",
    "OCVR_PP": "AA_VI_01_PP",
    "OCVR_FF": "AA_VI_02_FF",
    "OCVR_TH": "AA_VI_03_TH",
    "OCVR_DD": "AA_VI_04_DD",
    "OCVR_kk": "AA_VI_05_KK",
    "OCVR_CH": "AA_VI_06_CH",
    "OCVR_SS": "AA_VI_07_SS",
    "OCVR_nn": "AA_VI_08_nn",
    "OCVR_RR": "AA_VI_09_RR",
    "OCVR_aa": "AA_VI_10_aa",
    "OCVR_E": "AA_VI_11_E",
    "OCVR_I": "AA_VI_12_I",
    "OCVR_O": "AA_VI_13_O"
  }
}
```
