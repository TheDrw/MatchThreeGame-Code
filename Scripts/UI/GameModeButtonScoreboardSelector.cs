using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchThree.Data;
using UnityEngine.EventSystems;

namespace MatchThree.UI
{
    public class GameModeButtonScoreboardSelector : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] HighScores highScores;
        [SerializeField] ScoreboardHomeMenu scoreboard;

        public void OnPointerEnter(PointerEventData eventData)
        {
            scoreboard.gameObject.SetActive(true);
            scoreboard.InitializeScoreboardMenu(highScores.Results);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            scoreboard.gameObject.SetActive(false);
        }
    }
}