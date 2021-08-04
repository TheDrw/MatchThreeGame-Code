using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter.Effects
{
    public class BubbleTrailParticles : MonoBehaviour
    {
        [SerializeField] ParticleSystem mainTrail;
        [SerializeField] ParticleSystem sparkles;

        public void Play()
        {
            mainTrail.Play();
            sparkles.Play();
        }

        public void Stop()
        {
            mainTrail.Stop();
            sparkles.Stop();

            Destroy(gameObject, 1.5f);
        }
    }
}