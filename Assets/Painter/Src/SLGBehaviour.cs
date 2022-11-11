using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PainterSystem
{
    public class SLGBehaviour : MonoBehaviour
    {
        private bool isInitialized = false;
        private bool isPlaying = false;

        public void Initialize()
        {
            if (this.isInitialized) {
                return;
            }

            this.OnInitialize();

            this.isInitialized = true;
        }

        public void BeginPlay()
        {
            if (this.isPlaying) {
                return;
            }

            this.OnBeginPlay();

            this.isPlaying = true;
        }

        protected virtual void OnBeginPlay() { }
        protected virtual void OnInitialize() { }
    }
}
