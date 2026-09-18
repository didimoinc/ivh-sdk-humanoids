using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace PerCameraRenderPOCs.DoubleRenderPOC
{
    [RequireComponent(typeof(Camera))]
    public class DoubleRenderVisibility : MonoBehaviour
    {
        [SerializeField] private Transform avatarRoot;
        [SerializeField] private Material hiddenMaterial;

        [SerializeField] private SkinnedMeshRenderer sourceRenderer;
        [SerializeField] private string[] hiddenMaterialNames =
        {
            "ddmo_head_MAT",
            "ddmo_eyelashes_MAT",
            "ddmo_eyeLeft_MAT",
            "ddmo_eyeRight_MAT",
            "ddmo_mouth_MAT"
        };

        [SerializeField] private string thirdPersonLayerName;
        [SerializeField] private string firstPersonLayerName;

        private const string CloneName = "CC_Base_Body_FirstPerson";
        private readonly Dictionary<Camera, int> originalCameraMasks = new();
        private Camera firstPersonCamera;
        private SkinnedMeshRenderer firstPersonRenderer;
        private int sourceRendererOriginalLayer;
        private int thirdPersonLayer;
        private int firstPersonLayer;
        private bool isConfigured;

        private void Awake()
        {
            firstPersonCamera = GetComponent<Camera>();
            Configure();
        }

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;

            if (firstPersonCamera == null)
            {
                firstPersonCamera = GetComponent<Camera>();
            }

            Configure();
            ConfigureKnownCameras();
        }

        private void LateUpdate()
        {
            if (!isConfigured)
            {
                Configure();
            }

            SynchronizeBlendShapes();
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
            RestoreCameraMasks();
        }

        private void OnDestroy()
        {
            TearDown();
        }

        private void Configure()
        {
            if (isConfigured || avatarRoot == null || hiddenMaterial == null)
            {
                return;
            }

            thirdPersonLayer = LayerMask.NameToLayer(thirdPersonLayerName);
            firstPersonLayer = LayerMask.NameToLayer(firstPersonLayerName);

            if (thirdPersonLayer < 0 || firstPersonLayer < 0)
            {
                Debug.LogError(
                    $"First-person avatar layers are missing. Expected '{thirdPersonLayerName}' and " +
                    $"'{firstPersonLayerName}'.",
                    this);
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

            Material[] firstPersonMaterials = BuildFirstPersonMaterials(sourceRenderer.sharedMaterials);
            if (firstPersonMaterials == null)
            {
                return;
            }

            sourceRendererOriginalLayer = sourceRenderer.gameObject.layer;
            sourceRenderer.gameObject.layer = thirdPersonLayer;

            Transform oldClone = sourceRenderer.transform.Find(CloneName);
            if (oldClone != null)
            {
                Destroy(oldClone.gameObject);
            }

            GameObject cloneObject = new GameObject(CloneName);
            cloneObject.layer = firstPersonLayer;
            cloneObject.transform.SetParent(sourceRenderer.transform, false);

            firstPersonRenderer = cloneObject.AddComponent<SkinnedMeshRenderer>();
            CopyRendererSettings(sourceRenderer, firstPersonRenderer);
            firstPersonRenderer.sharedMaterials = firstPersonMaterials;
            firstPersonRenderer.shadowCastingMode = ShadowCastingMode.Off;

            isConfigured = true;
        }

        private SkinnedMeshRenderer FindSourceRenderer()
        {
            SkinnedMeshRenderer[] renderers = avatarRoot.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            foreach (SkinnedMeshRenderer skinnedMeshRenderer in renderers)
            {
                if (skinnedMeshRenderer.name == sourceRenderer.name && ContainsEveryHiddenMaterial(skinnedMeshRenderer.sharedMaterials))
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

        private Material[] BuildFirstPersonMaterials(Material[] originalMaterials)
        {
            Material[] result = (Material[])originalMaterials.Clone();
            HashSet<string> missingNames = new HashSet<string>(hiddenMaterialNames, StringComparer.OrdinalIgnoreCase);

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

        private static void CopyRendererSettings(
            SkinnedMeshRenderer source,
            SkinnedMeshRenderer destination)
        {
            destination.sharedMesh = source.sharedMesh;
            destination.rootBone = source.rootBone;
            destination.bones = source.bones;
            destination.quality = source.quality;
            destination.updateWhenOffscreen = source.updateWhenOffscreen;
            destination.skinnedMotionVectors = source.skinnedMotionVectors;
            destination.localBounds = source.localBounds;
            destination.receiveShadows = source.receiveShadows;
            destination.lightProbeUsage = source.lightProbeUsage;
            destination.reflectionProbeUsage = source.reflectionProbeUsage;
            destination.probeAnchor = source.probeAnchor;
            destination.allowOcclusionWhenDynamic = source.allowOcclusionWhenDynamic;
            destination.motionVectorGenerationMode = source.motionVectorGenerationMode;
            destination.renderingLayerMask = source.renderingLayerMask;
            destination.rendererPriority = source.rendererPriority;
        }

        private void SynchronizeBlendShapes()
        {
            if (sourceRenderer == null ||
                firstPersonRenderer == null ||
                sourceRenderer.sharedMesh == null)
            {
                return;
            }

            int blendShapeCount = sourceRenderer.sharedMesh.blendShapeCount;
            for (int i = 0; i < blendShapeCount; i++)
            {
                float sourceWeight = sourceRenderer.GetBlendShapeWeight(i);
                if (!Mathf.Approximately(firstPersonRenderer.GetBlendShapeWeight(i), sourceWeight))
                {
                    firstPersonRenderer.SetBlendShapeWeight(i, sourceWeight);
                }
            }
        }

        private void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam)
        {
            ConfigureCamera(cam);
        }

        private void ConfigureKnownCameras()
        {
            Camera[] cameras = Camera.allCameras;
            foreach (Camera camera in cameras)
            {
                ConfigureCamera(camera);
            }
        }

        private void ConfigureCamera(Camera camera)
        {
            if (camera == null || !isConfigured)
            {
                return;
            }

            if (!originalCameraMasks.ContainsKey(camera))
            {
                originalCameraMasks.Add(camera, camera.cullingMask);
            }

            int firstPersonBit = 1 << firstPersonLayer;
            int thirdPersonBit = 1 << thirdPersonLayer;

            if (camera == firstPersonCamera)
            {
                camera.cullingMask = (camera.cullingMask | firstPersonBit) & ~thirdPersonBit;
            }
            else
            {
                camera.cullingMask &= ~firstPersonBit;
            }
        }

        private void RestoreCameraMasks()
        {
            foreach (KeyValuePair<Camera, int> entry in originalCameraMasks)
            {
                if (entry.Key != null)
                {
                    entry.Key.cullingMask = entry.Value;
                }
            }

            originalCameraMasks.Clear();
        }

        private void TearDown()
        {
            RestoreCameraMasks();

            if (sourceRenderer != null)
            {
                sourceRenderer.gameObject.layer = sourceRendererOriginalLayer;
            }

            if (firstPersonRenderer != null)
            {
                Destroy(firstPersonRenderer.gameObject);
            }

            isConfigured = false;
        }
    }
}
