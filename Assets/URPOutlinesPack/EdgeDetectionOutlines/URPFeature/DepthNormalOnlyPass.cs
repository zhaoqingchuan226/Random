using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
namespace OutlinesPack
{
    public class DepthNormalOnlyPass : ScriptableRenderPass
    {
        private readonly RenderTargetHandle normals;

        private readonly Material normalsMaterial;
        private readonly List<ShaderTagId> shaderTagIdList;

        private FilteringSettings filteringSettings;
        private RenderStateBlock m_RenderStateBlock;

        public void SetDepthState(bool writeEnabled = true, CompareFunction function = CompareFunction.LessEqual)
        {
            m_RenderStateBlock.mask |= RenderStateMask.Depth;
            m_RenderStateBlock.depthState = new DepthState(writeEnabled, function);
        }

        public DepthNormalOnlyPass(RenderPassEvent renderPassEvent, LayerMask outlinesLayerMask, bool useDepthMask, string name = "_NormalsDepthTex")
        {
            filteringSettings = new FilteringSettings(RenderQueueRange.all, outlinesLayerMask);

            normalsMaterial = new Material(Shader.Find("OutlinesPack/NormalsDepth"));
            normalsMaterial.SetFloat("_UseDepthMask", useDepthMask ? 1 : 0);

            this.renderPassEvent = renderPassEvent;

            normals.Init(name);

            shaderTagIdList = new List<ShaderTagId>()
            {
                  new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("LightweightForward"),
                new ShaderTagId("SRPDefoultUnlit"),
                new ShaderTagId("DepthOnly")
            };

            m_RenderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);

            SetDepthState();
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            RenderTextureDescriptor normalsTextureDescriptor = cameraTextureDescriptor;
            normalsTextureDescriptor.colorFormat = RenderTextureFormat.ARGBFloat;
            normalsTextureDescriptor.msaaSamples = 1;

            cmd.GetTemporaryRT(normals.id, normalsTextureDescriptor, FilterMode.Point);
            ConfigureTarget(normals.Identifier());
            ConfigureClear(ClearFlag.All, Color.black);
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            DrawingSettings drawingSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, renderingData.cameraData.defaultOpaqueSortFlags);
            drawingSettings.overrideMaterial = normalsMaterial;
            drawingSettings.enableDynamicBatching = true;

            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings, ref m_RenderStateBlock);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(normals.id);

        }
    }
}