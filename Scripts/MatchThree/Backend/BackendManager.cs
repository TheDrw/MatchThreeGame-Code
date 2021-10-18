using System;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

namespace MatchThree.Backend
{
    public class BackendManager : MonoBehaviour
    {
        [SerializeField] GameObject backendLoader;

        public event Action OnPlayerLoginSuccess =  delegate{};

        void Start()
        {
            Login();
        }

        public void Login()
        {
            var request = new LoginWithCustomIDRequest
            {
                CustomId = SystemInfo.deviceUniqueIdentifier,
                CreateAccount = true
            };

            PlayFabClientAPI.LoginWithCustomID(request, OnSuccess, OnError);
        }

        void OnSuccess(LoginResult result)
        {
            Debug.Log("Success Login");
            backendLoader.SetActive(true);
            OnPlayerLoginSuccess();
        }

        void OnError(PlayFabError err)
        {
            Debug.LogError("ERR:  creatin/log in acct");
            Debug.LogError(err.GenerateErrorReport());
        }
    }
}