using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchThree.Core;
using MatchThree.Helpers;
using System;

namespace MatchThree.UI
{
    public class EndGameEarlyUI : MonoBehaviour
    {
        [SerializeField] GameObject endGameButton;
        [SerializeField] GameObject waitingText;

        GameManager GM = null;

        IEnumerator Start()
        {
            waitingText.SetActive(false);
            endGameButton.SetActive(false);
            yield return new WaitForSeconds(.5f);

            GM = GeneralHelp.GetGameManagerFromActiveScene();
            if(GM.CurrentGameState != GameState.Finished)
            {
                EnableCanvas();
            }
            else
            {
                DisableCanvas();
            }

            GameManager.OnGameEndingEarly += DisplayWaiting;
            GameManager.OnGameFinished += DisableCanvas;
        }


        private void OnDestroy()
        {
            GameManager.OnGameEndingEarly -= DisplayWaiting;
            GameManager.OnGameFinished -= DisableCanvas;
        }

        void DisplayButton()
        {
            endGameButton.SetActive(true);
        }

        void DisplayWaiting()
        {
            endGameButton.SetActive(false);
            waitingText.SetActive(true);
        }

        private void DisableCanvas()
        {
            GetComponent<Canvas>().enabled = false;
        }

        void EnableCanvas()
        {
            GetComponent<Canvas>().enabled = true;
            DisplayButton();
        }

        public void EndGameConfirmed()
        {
            GM.EndGameEarly();
        }
    }
}