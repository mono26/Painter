using PainterSystem;
using UnityEngine;

namespace StickerSystem
{
    public class Sticker : SLGBehaviour
    {
        [SerializeField]
        private float maxDistance = 1.0f;

        [SerializeField]
        private GameObject decalPFB = null;

        private Camera gameCamera = null;

        private void Start()
        {
            this.BeginPlay();            
        }

        private void Update()
        {
#if UNITY_EDITOR
            if (this.gameCamera != null) {
                Ray cameraRay = this.gameCamera.ScreenPointToRay(new Vector2(Screen.width/2, Screen.height/2));
                RaycastHit hit;
                
                Color rayColor = Physics.Raycast(cameraRay, out hit, this.maxDistance) ? Color.red : Color.green;

                Debug.DrawRay(this.transform.position, cameraRay.direction.normalized * this.maxDistance, rayColor);
            }
#endif

            if (!Input.GetKeyDown(KeyCode.T)) {
                return;
            }

            this.AddSticker();
        }

        protected override void OnBeginPlay()
        {
            base.OnInitialize();

            this.gameCamera = Camera.main;
        }

        private void AddSticker()
        {
            Debug.LogError("Try add sticker.");

            if (this.gameCamera == null || this.decalPFB == null) {
                return;
            }

            Ray cameraRay = this.gameCamera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;
            if (!Physics.Raycast(cameraRay, out hit, this.maxDistance)) {
                return;
            }

            Debug.LogError("Add sticker.");

            Vector3 stickerPosition = hit.point;
            Quaternion stickerRotation = Quaternion.LookRotation(hit.normal);

            Instantiate(this.decalPFB, stickerPosition, stickerRotation);
        }
    }
}
