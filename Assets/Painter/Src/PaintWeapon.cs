using Unity.FPS.Game;
using UnityEngine;

namespace PainterSystem
{
    public class PaintWeapon : WeaponController
    {
        [SerializeField]
        ParticleSystem paintParticlesComponent = null;

        protected override void Awake()
        {
            base.Awake();

            this.paintParticlesComponent = this.transform.FindRecursive("VFX_Paint").GetChild(0).GetComponent<ParticleSystem>();
        }

        protected override void Update()
        {
            base.Update();

            this.UpdateContinuousPaintVFX();
        }

        void UpdateContinuousPaintVFX()
        {
            if (m_WantsToShoot && m_CurrentAmmo >= 1f) {
                if (!this.paintParticlesComponent.isPlaying) {
                    this.paintParticlesComponent.Play();
                }
            }
            else if (this.paintParticlesComponent.isPlaying) {
                this.paintParticlesComponent.Stop();
            }
        }

        public override void UseAmmo(float amount)
        {
            m_LastTimeShot = Time.time;
        }
    }
}
