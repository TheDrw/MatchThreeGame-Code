using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;
using MatchThree.Data;

namespace MatchThree.UI
{
    public class PreloadBackendMenu : MonoBehaviour
    {
        [Header("Menus")]
        [SerializeField] GameObject playfabConfirmMenu;
        [SerializeField] GameObject noCloudSavesMenu;

        [Header("Etc")]
        [SerializeField] GameObject backendObject;

        bool isGoingToHome = false;

        private void Awake()
        {
            playfabConfirmMenu.SetActive(false);
            noCloudSavesMenu.SetActive(false);
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(0.5f);
            playfabConfirmMenu.SetActive(true);

            DataSave.OnDataLoadedFromServer += GoToHome;
        }

        private void OnDestroy()
        {
            DataSave.OnDataLoadedFromServer -= GoToHome;
        }

        public void ActivateBackend()
        {
            backendObject.SetActive(true);

            playfabConfirmMenu.SetActive(false);
        }

        public void DeclineBackend()
        {
            playfabConfirmMenu.SetActive(false);
            noCloudSavesMenu.SetActive(true);
        }

        public void GoToHome()
        {
            if (isGoingToHome) return;

            noCloudSavesMenu.SetActive(false);
            isGoingToHome = true;
            StartCoroutine(GoToHomeDelay());
        }

        IEnumerator GoToHomeDelay()
        {
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1, LoadSceneMode.Single);
        }
    }
}
