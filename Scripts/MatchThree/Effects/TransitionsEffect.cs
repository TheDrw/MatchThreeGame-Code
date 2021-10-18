using UnityEngine;
using System.Collections;
using System;

namespace MatchThree.Effects
{
    public class TransitionsEffect : MonoBehaviour
    {
        [SerializeField] Material transitionMat;
        readonly int FADE_ID = Shader.PropertyToID("_Fade");

        public event Action OnFadeInStarted = delegate { };
        public event Action OnFadeInFinished = delegate { };

        public event Action OnFadeOutStarted = delegate { };
        public event Action OnFadeOutFinished = delegate { };
        

        void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            Graphics.Blit(src, dest, transitionMat);
        }

        public void FadeOut(float duration, float delay)
        {
            StartCoroutine(FadeOutRoutine(duration, delay));
        }

        IEnumerator FadeOutRoutine(float duration, float delay)
        {
            OnFadeOutStarted();
            transitionMat.SetFloat(FADE_ID, 1f);

            if (delay > 0f)
            {
                yield return new WaitForSecondsRealtime(delay);
            }

            if (duration > float.Epsilon)
            {

                for (float start = 1f; start > 0; start = Mathf.MoveTowards(start, 0, (1 / duration) * Time.unscaledDeltaTime))
                {
                    transitionMat.SetFloat(FADE_ID, start);
                    yield return null;
                }
            }

            transitionMat.SetFloat(FADE_ID, 0f);
            enabled = false;
            OnFadeOutFinished();
        }

        public void FadeIn(float duration, float delay)
        {
            StartCoroutine(FadeInRoutine(duration, delay));
        }

        IEnumerator FadeInRoutine(float duration, float delay)
        {
            OnFadeInStarted();

            transitionMat.SetFloat(FADE_ID, 0f);

            if (delay > 0)
            {
                yield return new WaitForSecondsRealtime(delay);
            }

            if (duration > float.Epsilon)
            {
                for (float start = 0f; start < 1; start = Mathf.MoveTowards(start, 1, (1 / duration) * Time.unscaledDeltaTime))
                {
                    transitionMat.SetFloat(FADE_ID, start);
                    yield return null;
                }
            }

            transitionMat.SetFloat(FADE_ID, 1f);

            OnFadeInFinished();
        }
    }
}