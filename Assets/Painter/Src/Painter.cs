using UnityEngine;

namespace PainterSystem
{
    public class Painter : MonoBehaviour
    {
        public Color paintColor = Color.yellow;

        [SerializeField]
        protected float splashMinRadius = 0.05f;
        [SerializeField]
        protected float splashMaxRadius = 0.2f;
        [SerializeField]
        protected float splashStrength = 1;
        [SerializeField]
        protected float splashHardness = 1;

        public void Paint(PaintableRenderer paintable, Vector3 pos)
        {
            float radius = Random.Range(this.splashMinRadius, this.splashMaxRadius);

            Debug.DrawRay(pos, Vector3.up * radius, Color.green, 3.0f);
            Debug.DrawRay(pos, Vector3.right * radius, Color.red, 3.0f);
            Debug.DrawRay(pos, Vector3.forward * radius, Color.blue, 3.0f);

            PaintableWorld.Instance.Paint(paintable, pos, radius, this.splashHardness, this.splashStrength, this.paintColor);
        }
    }
}
