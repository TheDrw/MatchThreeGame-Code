using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MatchThree.Core;

namespace MatchThree.Scene
{
    public class SceneLoader : MonoBehaviour
    {
        private void Start()
        {
            GameManager.OnGameFinished += RestartScene;
        }

        private void OnDestroy()
        {
            GameManager.OnGameFinished -= RestartScene;
        }

        void RestartScene()
        {
            StartCoroutine(RestartSceneCO());
        }

        IEnumerator RestartSceneCO()
        {
            print("Restarting Scene in 3 seconds!");
            yield return new WaitForSeconds(3f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
        }

    }
}