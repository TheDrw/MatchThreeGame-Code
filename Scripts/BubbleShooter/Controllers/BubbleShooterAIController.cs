using System.Collections;
using UnityEngine;

namespace BubbleShooter.Controller
{
    public class BubbleShooterAIController : BubbleShooterController 
    {
        protected override void Start()
        {
            base.Start();

            StartCoroutine(Brainz());
        }

        // temp
        IEnumerator Brainz()
        {
            var waitUntilActive = new WaitUntil(() => isActive == true);
            yield return waitUntilActive;

            var oneSec = new WaitForSeconds(1f);
            var waitUntilShooterLoaded = new WaitUntil(() => shooter.IsLoaded);
            while (!IsDisposed)
            {
                yield return StupidAIDetermineShootRotation();

                ShootBubble();
                yield return oneSec;
                yield return waitUntilShooterLoaded;
            }
        }

        /// <summary>
        /// Determine rotation - completely 'random'.
        /// </summary>
        /// <returns></returns>
        IEnumerator StupidAIDetermineShootRotation()
        {
            int numOfSwitches = Random.Range(1, 5);
            for (int i = 0; i < numOfSwitches; i++)
            {

                float duration = Random.Range(.1f, 1f);
                if (Random.Range(0, 2) == 0)
                {
                    yield return DoAutoRotate(duration, ShootDirection.Left);
                }
                else
                {
                    yield return DoAutoRotate(duration, ShootDirection.Right);
                }

                SetShootDirection(ShootDirection.Idle);
                float extremeThinkingPause = Random.Range(.25f, 1.5f);
                yield return new WaitForSeconds(extremeThinkingPause);
            }
        }

        IEnumerator DoAutoRotate(float duration, ShootDirection dir)
        {
            SetShootDirection(dir);
            yield return new WaitForSeconds(duration);
        }
    }
}