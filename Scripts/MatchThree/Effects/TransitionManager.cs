using UnityEngine;

namespace MatchThree.Effects
{
    [RequireComponent(typeof(TransitionsEffect))]
    public class TransitionManager : MonoBehaviour
    {
        [SerializeField] TransitionsEffect transitionEffect = null;

        private void Awake()
        {
            transitionEffect = transitionEffect == null ?
                GetComponent<TransitionsEffect>() : transitionEffect;
            transitionEffect.enabled = false;
        }

        private void Start()
        {
            FadeOutTransition(.15f, .5f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="duration">Time it takes to transition. Values of 0 or lower will be 0.</param>
        /// <param name="delay">Delay before it starts. 0 or lower gives no delay.</param>
        public void FadeOutTransition(float duration, float delay)
        {
            transitionEffect.enabled = true;
            transitionEffect.FadeOut(duration, delay);
        }

        public void FadeInTransition(float duration, float delay)
        {
            transitionEffect.enabled = true;
            transitionEffect.FadeIn(duration, delay);
        }
    }
}