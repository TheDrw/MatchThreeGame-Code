using UnityEngine;

namespace MatchThree.Effects
{
    [RequireComponent(typeof(TransitionsEffect))]
    public class TransitionManager : MonoBehaviour
    {
        TransitionsEffect transitionEffect;

        private void Awake()
        {
            transitionEffect = GetComponent<TransitionsEffect>();
            transitionEffect.enabled = false;
        }

        private void Start()
        {
            FadeOutTransition(.15f, .5f);
        }

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