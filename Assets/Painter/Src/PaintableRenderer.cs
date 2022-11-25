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
        private string shaderTargetMaskTextureName = "_MaskTexture";

        [SerializeField]
        private Renderer rendererComponent = null;

        private uint id;

        public Material PaintableMaterialClone { get; private set; }

        // Painting
        public RenderTexture PaintedMaskTexture { get; private set; }
        public RenderTexture RuntimeMaskTexture { get; private set; }
        public RenderTexture FixedIslandsMaskTexture { get; private set; }

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

        private void OnDestroy()
        {
            Destroy(this.PaintableMaterialClone);

            Destroy(this.RuntimeMaskTexture);
            Destroy(this.FixedIslandsMaskTexture);
            Destroy(this.PaintedMaskTexture);

            Destroy(this.UVIslandsTexture);
        }

        private void OnDisable()
        {
            this.RuntimeMaskTexture.Release();
            this.PaintedMaskTexture.Release();
            this.FixedIslandsMaskTexture.Release();

            this.UVIslandsTexture.Release();
        }
        #endregion

        protected override void OnBeginPlay()
        {
            PaintableWorld.Instance.InitPaintable(this);
            this.PaintableMaterialClone.SetTexture(this.shaderTargetMaskTextureName, this.FixedIslandsMaskTexture);
            this.rendererComponent.material = this.PaintableMaterialClone;
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

            this.PaintableMaterialClone = new Material(this.rendererComponent.material);

            Texture mainTexture = this.rendererComponent.sharedMaterial.GetTexture(this.ShaderMainTextureName);
            int width = mainTexture != null ? mainTexture.width : 1024;
            int height = mainTexture != null ? mainTexture.height : 1024;

            this.RuntimeMaskTexture = new RenderTexture(width, height, 0) {
                useMipMap = false,
                filterMode = FilterMode.Bilinear,
            };
            this.RuntimeMaskTexture.name = $"Paint-RuntimeMaskTexture-{this.id}";

            this.PaintedMaskTexture = new RenderTexture(width, height, 0) {
                useMipMap = false,
                filterMode = FilterMode.Bilinear,
            };
            this.PaintedMaskTexture.name = $"Paint-PaintResultMaskTexture-{this.id}";

            this.FixedIslandsMaskTexture = new RenderTexture(this.PaintedMaskTexture.descriptor);
            this.FixedIslandsMaskTexture.name = $"Paint-FixedIslandsTexture-{this.id}";

            this.UVIslandsTexture = new RenderTexture(width, height, 0) {
                useMipMap = false,
                filterMode = FilterMode.Bilinear,
            };
            this.UVIslandsTexture.name = $"UVIslandsTexture-{this.id}";
        }

        public void Paint(Vector3 pos, Color color, float radius, float hardness, float strength)
        {
            PaintableWorld.Instance.Paint(this, pos, radius, hardness, strength, color);
        }
    }
}