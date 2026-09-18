using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace PerCameraRenderPOCs.SubmeshRenderFeaturePOC
{
    public class FirstPersonSubmeshRenderFeature : ScriptableRendererFeature
    {
        [SerializeField] private RenderPassEvent injectionPoint = RenderPassEvent.AfterRenderingOpaques;

        private SubmeshDrawPass pass;

        public override void Create()
        {
            pass = new SubmeshDrawPass { renderPassEvent = injectionPoint };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            Camera camera = renderingData.cameraData.camera;
            if (camera == null)
            {
                return;
            }

            PerCameraSubmeshVisibility visibility = camera.GetComponent<PerCameraSubmeshVisibility>();
            if (visibility == null || !visibility.isActiveAndEnabled)
            {
                return;
            }

            if (visibility.SourceRenderer == null || visibility.VisibleSubmeshes.Count == 0)
            {
                return;
            }

            pass.Setup(visibility);
            renderer.EnqueuePass(pass);
        }

        protected override void Dispose(bool disposing)
        {
            pass = null;
        }

        private sealed class SubmeshDrawPass : ScriptableRenderPass
        {
            private static readonly ShaderTagId LightModeTag = new("LightMode");
            private const string ProfilerTag = "First Person Submesh (body, no face)";

            private PerCameraSubmeshVisibility hider;
            private readonly Dictionary<Shader, int> forwardPassIndexCache = new();

            public void Setup(PerCameraSubmeshVisibility owner)
            {
                hider = owner;
            }

            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                SkinnedMeshRenderer renderer = hider != null ? hider.SourceRenderer : null;
                if (renderer == null)
                {
                    return;
                }

                CommandBuffer cmd = CommandBufferPool.Get(ProfilerTag);
                Material[] materials = renderer.sharedMaterials;

                foreach (int submesh in hider.VisibleSubmeshes)
                {
                    if (submesh < 0 || submesh >= materials.Length)
                    {
                        continue;
                    }

                    Material material = materials[submesh];
                    if (material == null)
                    {
                        continue;
                    }

                    int shaderPass = GetForwardPassIndex(material.shader);
                    if (shaderPass < 0)
                    {
                        continue;
                    }

                    cmd.DrawRenderer(renderer, material, submesh, shaderPass);
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }
            
            private int GetForwardPassIndex(Shader shader)
            {
                if (forwardPassIndexCache.TryGetValue(shader, out int cached))
                {
                    return cached;
                }

                int result = -1;
                int fallbackUnlit = -1;

                for (int i = 0; i < shader.passCount; i++)
                {
                    string lightMode = shader.FindPassTagValue(i, LightModeTag).name;
                    if (lightMode == "UniversalForward")
                    {
                        result = i;
                        break;
                    }

                    if (lightMode == "SRPDefaultUnlit" && fallbackUnlit < 0)
                    {
                        fallbackUnlit = i;
                    }
                }

                if (result < 0)
                {
                    result = fallbackUnlit >= 0 ? fallbackUnlit : 0;
                }

                forwardPassIndexCache[shader] = result;
                return result;
            }
        }
    }
}
