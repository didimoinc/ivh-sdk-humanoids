using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PerCameraRenderPOCs.SubmeshRenderFeaturePOC
{
    [RequireComponent(typeof(Camera))]
    public class PerCameraSubmeshVisibility : MonoBehaviour
    {
        [SerializeField] private Transform avatarRoot;
        [SerializeField] private string sourceRendererName = "CC_Base_Body";
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
        private readonly List<int> visibleSubmeshes = new();
        private bool suppressed;
        private bool configured;

        public SkinnedMeshRenderer SourceRenderer => sourceRenderer;
        public IReadOnlyList<int> VisibleSubmeshes => visibleSubmeshes;

        private void Awake()
        {
            firstPersonCamera = GetComponent<Camera>();
            Configure();
        }

        private void OnEnable()
        {
            if (firstPersonCamera == null)
            {
                firstPersonCamera = GetComponent<Camera>();
            }

            Configure();
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
            RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
            RestoreRenderer();
        }

        private void Configure()
        {
            if (configured || avatarRoot == null)
            {
                return;
            }

            sourceRenderer = FindSourceRenderer();
            if (sourceRenderer == null)
            {
                Debug.LogError(
                    $"Could not find a SkinnedMeshRenderer below '{avatarRoot.name}' containing the facial materials.",
                    this);
                return;
            }

            visibleSubmeshes.Clear();
            Material[] materials = sourceRenderer.sharedMaterials;
            HashSet<string> hidden = new HashSet<string>(hiddenMaterialNames, StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < materials.Length; i++)
            {
                string materialName = materials[i] != null ? StripInstanceSuffix(materials[i].name) : null;
                if (materialName == null || !hidden.Contains(materialName))
                {
                    visibleSubmeshes.Add(i);
                }
            }

            sourceRenderer.updateWhenOffscreen = true;
            configured = true;
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
                    if (material != null && string.Equals(StripInstanceSuffix(material.name), hiddenName,
                            StringComparison.OrdinalIgnoreCase))
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

        private static string StripInstanceSuffix(string name)
        {
            const string instanceSuffix = " (Instance)";
            return name.EndsWith(instanceSuffix, StringComparison.Ordinal)
                ? name.Substring(0, name.Length - instanceSuffix.Length)
                : name;
        }

        private void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam)
        {
            if (!configured)
            {
                Configure();
            }

            if (cam != firstPersonCamera || sourceRenderer == null)
            {
                return;
            }
            
            sourceRenderer.forceRenderingOff = true;
            suppressed = true;
        }

        private void OnEndCameraRendering(ScriptableRenderContext context, Camera cam)
        {
            if (cam != firstPersonCamera)
            {
                return;
            }

            RestoreRenderer();
        }

        private void RestoreRenderer()
        {
            if (suppressed && sourceRenderer != null)
            {
                sourceRenderer.forceRenderingOff = false;
            }

            suppressed = false;
        }
    }
}
