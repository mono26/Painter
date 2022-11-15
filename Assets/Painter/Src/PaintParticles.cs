using System.Collections.Generic;
using UnityEngine;

namespace PainterSystem
{
    public class PaintParticles : Painter
    {
        ParticleSystem particlesSystemComponent;
        List<ParticleCollisionEvent> collisionEvents;

        private void Start()
        {
            this.particlesSystemComponent = GetComponent<ParticleSystem>();
            this.collisionEvents = new List<ParticleCollisionEvent>();

            ParticleSystemRenderer particlesRenderer = this.particlesSystemComponent.GetComponent<ParticleSystemRenderer>();
            particlesRenderer.material.color = this.paintColor;
        }

        private void OnParticleCollision(GameObject other)
        {
            PaintableRenderer paintableComponent = other.GetComponent<PaintableRenderer>();
            if (paintableComponent == null) {
                return;
            }

            int numCollisionEvents = particlesSystemComponent.GetCollisionEvents(other, collisionEvents);
            for (int i = 0; i < numCollisionEvents; i++) {
                Vector3 pos = collisionEvents[i].intersection;
                this.Paint(paintableComponent, pos);
            }
        }
    }
}
