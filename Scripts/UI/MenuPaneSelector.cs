using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace MatchThree.UI
{
    public class MenuPaneSelector : MonoBehaviour
    {
        [SerializeField] bool activateFirstMenuOnEnable = true;
        [SerializeField] List<GameObject> menus;

        bool canSelect = true;
        int currIdx = 0;

        static readonly float duration = .25f;
        static float screenWidth = 0f;

        private void Awake()
        {
            foreach(var gameModeMenu in menus)
            {
                gameModeMenu.SetActive(false);
            }
        }

        private void OnEnable()
        {
            screenWidth = Screen.currentResolution.width;
            if (activateFirstMenuOnEnable)
            {
                menus[currIdx].SetActive(true);
            }
        }

        public void MoveMenuRight()
        {
            if (!canSelect) return;

            canSelect = false;
            var currMenu = menus[currIdx];
            currMenu.GetComponent<RectTransform>()
                .DOLocalMoveX(-screenWidth, duration)
                .OnComplete(()=> currMenu.SetActive(false));

            currIdx++;
            if (currIdx >= menus.Count) currIdx = 0;

            var newMenu = menus[currIdx];
            var newMenuRect = newMenu.GetComponent<RectTransform>();
            newMenuRect.anchoredPosition = new Vector2(screenWidth, newMenuRect.anchoredPosition.y);
            newMenu.SetActive(true);
            newMenuRect.DOLocalMoveX(0f, duration)
                .OnComplete(() => canSelect = true);
        }

        public void MoveMenuLeft()
        {
            if (!canSelect) return;

            canSelect = false;
            var currMenu = menus[currIdx];
            currMenu.GetComponent<RectTransform>()
                .DOLocalMoveX(screenWidth, duration)
                .OnComplete(() => currMenu.SetActive(false));

            currIdx--;
            if (currIdx < 0) currIdx = menus.Count - 1;

            var newMenu = menus[currIdx];
            var newMenuRect = newMenu.GetComponent<RectTransform>();
            newMenuRect.anchoredPosition = new Vector2(-screenWidth, newMenuRect.anchoredPosition.y);
            newMenu.SetActive(true);
            newMenuRect.DOLocalMoveX(0f, duration)
                .OnComplete(() => canSelect = true);
        }

    }
}