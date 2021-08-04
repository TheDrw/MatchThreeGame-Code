using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchThree.Core;
using BubbleShooter.Effects;

namespace BubbleShooter.Core
{
    public class Bubble : MonoBehaviour
    {
        BubbleTrailParticles bubbleTrailParticles = null;

        public bool IsShooting { get; private set; } = false;
        Vector3 dir = default;

        public int LastHitID { get; private set; }

        private void OnDestroy()
        {
        }

        public void AttachTrail(BubbleTrailParticles trail)
        {
            bubbleTrailParticles = Instantiate(trail, transform.position, Quaternion.identity, transform)
                .GetComponent<BubbleTrailParticles>(); ;
        }

        public void Stop()
        {
            if(IsShooting)
            {
                if (bubbleTrailParticles)
                {
                    bubbleTrailParticles.Stop();
                    bubbleTrailParticles.gameObject.transform.SetParent(null);
                }

                IsShooting = false;
            }
        }

        public void Shoot(Vector3 dir, float speed)
        {
            if(!IsShooting)
            {
                if (bubbleTrailParticles)
                {
                    bubbleTrailParticles.Play();
                }

                IsShooting = true;
                this.dir = dir;
                StartCoroutine(ShootCoroutine(speed));
            }
        }

        IEnumerator ShootCoroutine(float speed)
        {
            while(IsShooting)
            {
                transform.SetPositionAndRotation(transform.position + dir * (Time.deltaTime * speed), Quaternion.identity);
                yield return null;
            }
        }

        /// <summary>
        /// Remembers the instance ID of what we last hit. Prevents mulitple collision checks
        /// with the same object.
        /// </summary>
        /// <param name="instanceID"></param>
        public void CollidedWith(int instanceID) 
        {
            LastHitID = instanceID;
        }

        public void Bounce()
        {
            dir.x = -dir.x;
        }
    }
}