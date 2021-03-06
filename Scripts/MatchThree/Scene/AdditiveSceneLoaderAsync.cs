using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MatchThree.Scene
{
    public class AdditiveSceneLoaderAsync : MonoBehaviour
    {
        [Scene] 
        [SerializeField] string additiveScene;

        private IEnumerator Start()
        {
#if UNITY_EDITOR
            if (additiveScene == string.Empty)
            {
                Debug.LogError("ERR: Missing additive scene.", this);
            }
#endif

            yield return SceneManager.LoadSceneAsync(additiveScene, LoadSceneMode.Additive);
        }
    }
}