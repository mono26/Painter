using UnityEngine;
using UnityEngine.Rendering;

namespace PainterSystem
{
    public class PaintableWorld : Singleton<PaintableWorld>
    {
        #region Static stuff.
        private static uint paintableId = 0;
        private static uint GetNextPaintableID() => ++paintableId;
        #endregion

        public Shader shaderFixIslands;
        public Shader shaderMarkIslands;
        public Shader shaderPaintUV;

        private int positionID = Shader.PropertyToID("_PainterPosition");
        private int hardnessID = Shader.PropertyToID("_Hardness");
        private int strengthID = Shader.PropertyToID("_Strength");
        private int radiusID = Shader.PropertyToID("_Radius");
        private int colorID = Shader.PropertyToID("_PainterColor");
        private int textureID = Shader.PropertyToID("_MainTex");
        private int uvOffsetID = Shader.PropertyToID("_OffsetUV");
        private int uvIslandsID = Shader.PropertyToID("_UVIslands");

        Material materialFixIslands = null;
        Material materialMarkIslands = null;
        Material materialPaintUV = null;

        CommandBuffer commandBuffer;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this.materialPaintUV = new Material(shaderPaintUV);
            this.materialFixIslands = new Material(shaderFixIslands);
            this.materialMarkIslands = new Material(shaderMarkIslands);

            this.commandBuffer = new CommandBuffer();
            this.commandBuffer.name = "CommmandBuffer - " + gameObject.name;
        }

        public void InitPaintable(PaintableRenderer paintable)
        {
            if (paintable == null) {
                return;
            }

            paintable.SetId(PaintableWorld.GetNextPaintableID());

            RenderTexture runtimeRT = paintable.RuntimeTexture;
            RenderTexture paintedRT = paintable.PaintedTexture;
            RenderTexture fixedIslandsRT = paintable.FixedIslandsTexture;
            RenderTexture uvIslandsRT = paintable.FixedIslandsTexture;

            Renderer rendererComponent = paintable.RendererComponent;

            // Blit the original content of the texture in the runtimeRT.
            this.commandBuffer.SetRenderTarget(runtimeRT);
            this.commandBuffer.SetRenderTarget(paintedRT);
            this.commandBuffer.Blit(runtimeRT, paintedRT);

            // Create UV islandsRT.
            this.commandBuffer.SetRenderTarget(uvIslandsRT);
            this.commandBuffer.DrawRenderer(rendererComponent, this.materialMarkIslands);

            // Fix the islands of the paintedRT.
            this.materialFixIslands.SetFloat(uvOffsetID, paintable.ShaderExtendIslandOffset);
            this.materialFixIslands.SetTexture(uvIslandsID, uvIslandsRT);
            this.commandBuffer.SetRenderTarget(fixedIslandsRT);
            this.commandBuffer.Blit(paintedRT, fixedIslandsRT, this.materialFixIslands);

            Graphics.ExecuteCommandBuffer(this.commandBuffer);
            this.commandBuffer.Clear();
        }

        public void Paint(PaintableRenderer paintable, Vector3 pos, float radius = 1f, float hardness = .5f, float strength = .5f, Color? color = null)
        {
            if (paintable == null) {
                return;
            }

            RenderTexture runtimeRT = paintable.RuntimeTexture;
            RenderTexture paintedRT = paintable.PaintedTexture;
            RenderTexture fixedIslandsRT = paintable.FixedIslandsTexture;
            RenderTexture uvIslandsRT = paintable.FixedIslandsTexture;

            Renderer rendererComponent = paintable.RendererComponent;

            this.materialPaintUV.SetVector(this.positionID, pos);
            this.materialPaintUV.SetFloat(this.hardnessID, hardness);
            this.materialPaintUV.SetFloat(this.strengthID, strength);
            this.materialPaintUV.SetFloat(this.radiusID, radius);
            this.materialPaintUV.SetTexture(this.textureID, paintedRT);
            this.materialPaintUV.SetColor(colorID, color ?? Color.red);

            this.materialFixIslands.SetFloat(uvOffsetID, paintable.ShaderExtendIslandOffset);
            this.materialFixIslands.SetTexture(uvIslandsID, uvIslandsRT);

            // Create the new pain data in a texture.
            this.commandBuffer.SetRenderTarget(runtimeRT);
            this.commandBuffer.DrawRenderer(rendererComponent, this.materialPaintUV);

            // Add the new paint to the old painted texture.
            this.commandBuffer.SetRenderTarget(paintedRT);
            this.commandBuffer.Blit(runtimeRT, paintedRT);

            // Fix the painted texture islands.
            this.commandBuffer.SetRenderTarget(fixedIslandsRT);
            this.commandBuffer.Blit(paintedRT, fixedIslandsRT, this.materialFixIslands);

            Graphics.ExecuteCommandBuffer(this.commandBuffer);
            this.commandBuffer.Clear();
        }
    }
}
