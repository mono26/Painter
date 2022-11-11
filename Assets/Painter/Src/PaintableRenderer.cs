using System;
using UnityEngine;

namespace PainterSystem
{
    [RequireComponent(typeof(Renderer))]
    public class PaintableRenderer : SLGBehaviour
    {
        [SerializeField]
        private float shaderExtendIslandOffset = 1;
        [SerializeField]
        private string shaderMainTextureName = "_BaseMap";
        [SerializeField]
        private string shaderTargetTextureName = "_MaskTexture";

        [SerializeField]
        private Renderer rendererComponent = null;

        private uint id;

        private Material materialClone = null;

        public RenderTexture FixedIslandsTexture { get; private set; }
        public RenderTexture PaintedTexture { get; private set; }
        public RenderTexture RuntimeTexture { get; private set; }
        public RenderTexture UVIslandsTexture { get; private set; }

        public float ShaderExtendIslandOffset => this.shaderExtendIslandOffset;
        public string ShaderMainTextureName => this.shaderMainTextureName;

        public Renderer RendererComponent => this.rendererComponent;

        public void SetId(uint id)
        {
            this.id = id;
        }

        #region Unity calls.
        private void Awake()
        {
            this.Initialize();
        }

        private void Start()
        {
            this.OnBeginPlay();
        }

        private void OnDisable()
        {
            this.RuntimeTexture.Release();
            this.PaintedTexture.Release();
            this.FixedIslandsTexture.Release();
            this.UVIslandsTexture.Release();
        }
        #endregion

        protected override void OnBeginPlay()
        {
            PaintableWorld.Instance.InitPaintable(this);
            this.materialClone.SetTexture(this.shaderTargetTextureName, this.FixedIslandsTexture);
            this.rendererComponent.material = this.materialClone;
        }

        protected override void OnInitialize()
        {
            if (this.rendererComponent == null) {
                this.rendererComponent = GetComponent<Renderer>();
                if (this.rendererComponent == null) {
                    Debug.LogError("No renderer component found.");
                    return;
                }
            }

            this.materialClone = new Material(this.rendererComponent.material);

            Texture mainTexture = this.rendererComponent.sharedMaterial.GetTexture(this.ShaderMainTextureName);
            int width = mainTexture != null ? mainTexture.width : 1024;
            int height = mainTexture != null ? mainTexture.height : 1024;

            this.RuntimeTexture = new RenderTexture(width, height, 0) {
                useMipMap = false,
                filterMode = FilterMode.Bilinear,
            };
            this.RuntimeTexture.name = $"Paint-RuntimeTexture-{this.id}";

            this.PaintedTexture = new RenderTexture(width, height, 0) {
                useMipMap = false,
                filterMode = FilterMode.Bilinear,
            };
            this.PaintedTexture.name = $"Paint-PaintResultTexture-{this.id}";

            this.FixedIslandsTexture = new RenderTexture(this.PaintedTexture.descriptor);
            this.FixedIslandsTexture.name = $"Paint-FixedIslandsTexture-{this.id}";

            this.UVIslandsTexture = new RenderTexture(width, height, 0) {
                useMipMap = false,
                filterMode = FilterMode.Bilinear,
            };
            this.UVIslandsTexture.name = $"Paint-UVIslandsTexture-{this.id}";
        }
    }
}