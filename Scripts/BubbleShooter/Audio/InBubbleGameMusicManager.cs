using System.Collections;
using System.Collections.Generic;
using BubbleShooter.Core;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

namespace MatchThree.Audio
{
    /// <summary>
    /// https://forum.unity.com/threads/handling-audioclip-loading-when-to-unload-audioclips.791270/
    /// https://docs.unity3d.com/Packages/com.unity.addressables@1.11/manual/AddressableAssetsAsyncOperationHandle.html
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class InBubbleGameMusicManager : MonoBehaviour
    {
        [SerializeField] AssetReference mainMusicRef;
        [SerializeField] AssetReference wonMusicRef;
        [SerializeField] AssetReference lostMusicRef;

        AudioSource audioSource;

        Queue<AsyncOperationHandle<AudioClip>> handleRefs = new Queue<AsyncOperationHandle<AudioClip>>();

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            audioSource.Stop();
            audioSource.loop = true;
        }

        private IEnumerator Start()
        {
            BubbleGameManager.OnGameStart += PlayMainMusic;
            BubbleGameManager.OnPlayerWon += PlayWonMusic;
            BubbleGameManager.OnPlayerLost += PlayLoseMusic;

            yield return LoadMusic(mainMusicRef);
        }

        private void OnDisable()
        {
            Addressables.Release(handleRefs.Dequeue());
        }

        private void OnDestroy()
        {
            BubbleGameManager.OnGameStart -= PlayMainMusic;
            BubbleGameManager.OnPlayerWon -= PlayWonMusic;
            BubbleGameManager.OnPlayerLost -= PlayLoseMusic;
        }

        void PlayMainMusic()
        {
            audioSource.Play();
        }

        void PlayWonMusic()
        {
            StartCoroutine(PlayMusicRoutine(wonMusicRef));
        }

        void PlayLoseMusic()
        {
            StartCoroutine(PlayMusicRoutine(lostMusicRef));
        }

        IEnumerator PlayMusicRoutine(AssetReference musicRef)
        {
            audioSource.Stop();
            Addressables.Release(handleRefs.Dequeue()); // release prev music

            yield return new WaitForSeconds(1f);
            
            yield return LoadMusic(musicRef);

            yield return new WaitForSeconds(1f);
            audioSource.Play();
        }

        IEnumerator LoadMusic(AssetReference musicRef)
        {
            audioSource.clip = null;

            var loadAudioClipHandle = musicRef.LoadAssetAsync<AudioClip>();
            var waitHandle = new WaitUntil(() => loadAudioClipHandle.IsDone);
            yield return waitHandle;
            
            if (loadAudioClipHandle.Status == AsyncOperationStatus.Succeeded)
            {
                var result = loadAudioClipHandle.Result;
                audioSource.clip = result;
                handleRefs.Enqueue(loadAudioClipHandle);
            }
            else if(loadAudioClipHandle.Status == AsyncOperationStatus.Failed)
            {
                Debug.LogError($"ERR: Loading {musicRef.SubObjectName} failed to load " +
                    $"initialized or something happened", this);
            }
            else if(loadAudioClipHandle.Status == AsyncOperationStatus.None)
            {
                Debug.LogError($"ERR: Loading {musicRef.SubObjectName} failed something. Async Handle " +
                    $"status ended with None.", this);
            }
        }
    }
}