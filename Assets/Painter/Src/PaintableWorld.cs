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

        [SerializeField]
        public Shader shaderFixIslands;
        [SerializeField]
        public Shader shaderMarkIslands;
        [SerializeField]
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
        Material materialPaintTexture = null;
        Material materialStickerTexture = null;

        CommandBuffer commandBuffer;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            this.materialPaintTexture = new Material(this.shaderPaintUV);
            this.materialFixIslands = new Material(this.shaderFixIslands);
            this.materialMarkIslands = new Material(this.shaderMarkIslands);

            this.commandBuffer = new CommandBuffer();
            this.commandBuffer.name = "CommmandBuffer - " + this.gameObject.name;
        }

        public void InitPaintable(PaintableRenderer paintable)
        {
            if (paintable == null) {
                return;
            }

            paintable.SetId(PaintableWorld.GetNextPaintableID());

            RenderTexture runtimeRT = paintable.RuntimeMaskTexture;
            RenderTexture paintedRT = paintable.PaintedMaskTexture;
            RenderTexture fixedIslandsRT = paintable.FixedIslandsMaskTexture;

            RenderTexture uvIslandsRT = paintable.FixedIslandsMaskTexture;

            Renderer rendererComponent = paintable.RendererComponent;

            // Blit the original content of the texture in the runtimeRT.
            this.commandBuffer.SetRenderTarget(runtimeRT);
            this.commandBuffer.SetRenderTarget(paintedRT);
            // this.commandBuffer.Blit(runtimeRT, paintedRT);

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

        public void Paint(PaintableRenderer paintable, Vector3 pos, float radius, float hardness, float strength, Color color)
        {
            if (paintable == null) {
                return;
            }

            RenderTexture runtimeMaskRT = paintable.RuntimeMaskTexture;
            RenderTexture paintedMaskRT = paintable.PaintedMaskTexture;
            RenderTexture fixedIslandsMaskRT = paintable.FixedIslandsMaskTexture;

            RenderTexture uvIslandsRT = paintable.FixedIslandsMaskTexture;

            Renderer rendererComponent = paintable.RendererComponent;

            this.materialPaintTexture.SetVector(this.positionID, pos);
            this.materialPaintTexture.SetFloat(this.hardnessID, hardness);
            this.materialPaintTexture.SetFloat(this.strengthID, strength);
            this.materialPaintTexture.SetFloat(this.radiusID, radius);
            this.materialPaintTexture.SetTexture(this.textureID, paintedMaskRT);
            this.materialPaintTexture.SetColor(colorID, color);

            this.materialFixIslands.SetFloat(uvOffsetID, paintable.ShaderExtendIslandOffset);
            this.materialFixIslands.SetTexture(uvIslandsID, uvIslandsRT);

            // Create the new paint data in a texture.
            this.commandBuffer.SetRenderTarget(runtimeMaskRT);
            this.commandBuffer.DrawRenderer(rendererComponent, this.materialPaintTexture);

            // Add the new paint to the old painted texture.
            this.commandBuffer.SetRenderTarget(paintedMaskRT);
            this.commandBuffer.Blit(runtimeMaskRT, paintedMaskRT);

            // Fix the painted texture islands.
            this.commandBuffer.SetRenderTarget(fixedIslandsMaskRT);
            this.commandBuffer.Blit(paintedMaskRT, fixedIslandsMaskRT, this.materialFixIslands);

            Graphics.ExecuteCommandBuffer(this.commandBuffer);
            this.commandBuffer.Clear();
        }
    }
}
