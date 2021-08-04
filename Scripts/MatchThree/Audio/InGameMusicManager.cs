using System.Collections;
using MatchThree.Core;
using UnityEngine;

namespace MatchThree.Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class InGameMusicManager : MonoBehaviour
    {
        AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            if (audioSource.clip == null)
            { 
#if UNITY_EDITOR
                Debug.LogError("ERR: Missing music.", this);
#endif
            }

            audioSource.Stop();
            audioSource.loop = true;
        }

        private void Start()
        {
            GameManager.OnGameStart += PlayMusic;
        }

        private void OnDestroy()
        {
            GameManager.OnGameStart -= PlayMusic;
        }

        void PlayMusic()
        {
            audioSource.Play();
        }
    }
}