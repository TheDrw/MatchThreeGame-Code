using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchThree.Data;
using TMPro;
using UnityEngine.EventSystems;

namespace MatchThree.UI
{
    public class PlayerChoiceUI : MonoBehaviour, ISubmitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] PlayerChoice playerChoice;
        [SerializeField] TMP_Text choice;


        private void OnEnable()
        {
            choice.text = $"{playerChoice.CurrentChoice}";
        }

        public void GetNext()
        {
            playerChoice.NextChoice();
            choice.text = $"{playerChoice.CurrentChoice}";
        }

        public void OnSubmit(BaseEventData eventData)
        {
            //throw new System.NotImplementedException();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //throw new System.NotImplementedException();
        }

        public void OnPointerUp(PointerEventData eventData)
        {

        }
    }
}