using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MatchThree.UI.Effects
{
    [RequireComponent(typeof(TMP_Text))]
    public class TypewriterEffect : MonoBehaviour
    {
        [Header("Settings")]
        [Tooltip("The delay before starting when the text popups.")]
        [SerializeField] float delay = 0f;

        [Tooltip("Increase speed of typewriter. Generally takes 2 seconds to finish if value is 1.")]
        [SerializeField] float speedMultiplier = 1f;

        [SerializeField] bool resetOnDisable = true;

        [Header("Audio")]
        [SerializeField] AudioClip typeSoundSFX;

        IEnumerator typewriteCO;
        bool ranOnce = false;

        private void OnValidate()
        {
            speedMultiplier = Mathf.Clamp(speedMultiplier, 0.01f, 25f);
        }

        private void Awake()
        {
            typewriteCO = TypewriteCoroutine();
        }

        private void OnEnable()
        {
            if (resetOnDisable && ranOnce) return;

            StartCoroutine(typewriteCO);
        }

        private void OnDisable()
        {
            StopCoroutine(typewriteCO);

            if (resetOnDisable && ranOnce)
            {
                typewriteCO = null;
                typewriteCO = TypewriteCoroutine();

                ranOnce = false;
            }
        }
        
        IEnumerator TypewriteCoroutine()
        {
            var words = GetComponent<TMP_Text>();
            var audioSource = typeSoundSFX == null ? null : GetComponent<AudioSource>();


            words.maxVisibleCharacters = 0;
            yield return new WaitForSecondsRealtime(delay);

            int maxCharCount = words.text.Length;

            words.ForceMeshUpdate();

            float typewritterSpeedPerCharacter = 2f / (((float)maxCharCount) * speedMultiplier);
            var step = new WaitForSecondsRealtime(typewritterSpeedPerCharacter);

            for (int currCharCount = 1; currCharCount <= maxCharCount; currCharCount++)
            {
                words.maxVisibleCharacters = currCharCount;

                if (audioSource)
                {
                    audioSource.clip = typeSoundSFX;
                    audioSource.Play();
                }

                yield return step;
            }

            ranOnce = true;
        }
    }
}