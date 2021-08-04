using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace MatchThree.UI
{
    public class ControllerUI : MonoBehaviour
    {
        [SerializeField] GameObject finishedOverlay;
        [SerializeField] TMP_Text finTimeText;
        [SerializeField] TMP_Text rankText;
        [SerializeField] Image rankImage;
        [SerializeField] GameObject initializingBoardTextObject;

        private void Awake()
        {
            finishedOverlay.SetActive(false);
            initializingBoardTextObject.SetActive(false);
        }

        private IEnumerator Start()
        {
            yield return new WaitForSeconds(.3f);
            initializingBoardTextObject.SetActive(true);
        }

        public void ShowFinishedOverlay(float time, int rank)
        {
            finishedOverlay.SetActive(true);
            finTimeText.text = $"Fin : {time.ToString(".000")}";
            rankText.text = $"{rank}";

            rankImage.enabled = rank == 1;
        }

        public void HideInitializingBoardText()
        {
            initializingBoardTextObject.SetActive(false);
        }
    }
}