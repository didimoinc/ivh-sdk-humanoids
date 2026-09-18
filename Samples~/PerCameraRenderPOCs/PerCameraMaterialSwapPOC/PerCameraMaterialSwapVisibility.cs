using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PerCameraRenderPOCs.PerCameraMaterialSwapPOC
{
    [RequireComponent(typeof(Camera))]
    public class PerCameraMaterialSwapVisibility : MonoBehaviour
    {
        [SerializeField] private Transform avatarRoot;
        [SerializeField] private Material hiddenMaterial;

        [SerializeField] private string sourceRendererName;
        [SerializeField] private string[] hiddenMaterialNames =
        {
            "ddmo_head_MAT",
            "ddmo_eyelashes_MAT",
            "ddmo_eyeLeft_MAT",
            "ddmo_eyeRight_MAT",
            "ddmo_mouth_MAT"
        };

        private Camera firstPersonCamera;
        private SkinnedMeshRenderer sourceRenderer;
        private Material[] originalMaterials;
        private Material[] firstPersonMaterials;
        private int activeSwapDepth;

        private void Awake()
        {
            firstPersonCamera = GetComponent<Camera>();
            Configure();
        }

        private void OnEnable()
        {
            firstPersonCamera = GetComponent<Camera>();
            Configure();
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
            RestoreOriginalMaterials();
        }

        private void OnDestroy()
        {
            RestoreOriginalMaterials();
        }

        private void Configure()
        {
            if (sourceRenderer != null || avatarRoot == null || hiddenMaterial == null)
            {
                return;
            }

            sourceRenderer = FindSourceRenderer();
            if (sourceRenderer == null)
            {
                Debug.LogError(
                    $"Could not find the Business Female face/body SkinnedMeshRenderer below '{avatarRoot.name}'.",
                    this);
                return;
            }

            originalMaterials = sourceRenderer.sharedMaterials;
            firstPersonMaterials = BuildFirstPersonMaterials(originalMaterials);

            if (firstPersonMaterials == null)
            {
                sourceRenderer = null;
                originalMaterials = null;
            }
        }

        private SkinnedMeshRenderer FindSourceRenderer()
        {
            SkinnedMeshRenderer[] renderers = avatarRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            foreach (SkinnedMeshRenderer skinnedMeshRenderer in renderers)
            {
                if (skinnedMeshRenderer.name == sourceRendererName && ContainsEveryHiddenMaterial(skinnedMeshRenderer.sharedMaterials))
                {
                    return skinnedMeshRenderer;
                }
            }

            foreach (SkinnedMeshRenderer skinnedMeshRenderer in renderers)
            {
                if (ContainsEveryHiddenMaterial(skinnedMeshRenderer.sharedMaterials))
                {
                    return skinnedMeshRenderer;
                }
            }

            return null;
        }

        private bool ContainsEveryHiddenMaterial(Material[] materials)
        {
            foreach (string hiddenName in hiddenMaterialNames)
            {
                bool found = false;

                foreach (Material material in materials)
                {
                    if (material != null && MaterialNameEquals(material.name, hiddenName))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                {
                    return false;
                }
            }

            return true;
        }

        private Material[] BuildFirstPersonMaterials(Material[] materials)
        {
            Material[] result = (Material[])materials.Clone();
            HashSet<string> missingNames =
                new HashSet<string>(hiddenMaterialNames, StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < result.Length; i++)
            {
                Material material = result[i];
                if (material == null)
                {
                    continue;
                }

                foreach (string hiddenName in hiddenMaterialNames)
                {
                    if (!MaterialNameEquals(material.name, hiddenName))
                    {
                        continue;
                    }

                    result[i] = hiddenMaterial;
                    missingNames.Remove(hiddenName);
                    break;
                }
            }

            if (missingNames.Count == 0)
            {
                return result;
            }

            Debug.LogError(
                $"The selected renderer is missing facial material slots: {string.Join(", ", missingNames)}.",
                this);
            return null;
        }

        private static bool MaterialNameEquals(string actualName, string expectedName)
        {
            const string instanceSuffix = " (Instance)";
            if (actualName.EndsWith(instanceSuffix, StringComparison.Ordinal))
            {
                actualName = actualName.Substring(0, actualName.Length - instanceSuffix.Length);
            }

            return string.Equals(actualName, expectedName, StringComparison.OrdinalIgnoreCase);
        }

        private void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam)
        {
            if (cam != firstPersonCamera || sourceRenderer == null || firstPersonMaterials == null)
            {
                return;
            }

            if (activeSwapDepth++ == 0)
            {
                sourceRenderer.sharedMaterials = firstPersonMaterials;
            }
        }

        private void OnEndCameraRendering(ScriptableRenderContext context, Camera cam)
        {
            if (cam != firstPersonCamera || activeSwapDepth == 0)
            {
                return;
            }

            activeSwapDepth--;
            if (activeSwapDepth == 0)
            {
                RestoreOriginalMaterials();
            }
        }

        private void RestoreOriginalMaterials()
        {
            activeSwapDepth = 0;

            if (sourceRenderer != null && originalMaterials != null)
            {
                sourceRenderer.sharedMaterials = originalMaterials;
            }
        }
    }
}
