using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

namespace MatchThree.UI
{
    public class MenuVerticalHide : MonoBehaviour
    {
        [SerializeField] RectTransform menu;

        static float duration = .25f;
        bool canMove = true;

        public void HideMenuDown()
        {
            if (!canMove) return;

            canMove = false;
            menu.DOLocalMoveY(-menu.rect.height, duration)
                .OnComplete(() => canMove = true);
        }

        public void ShowMenuUp()
        {
            if (!canMove) return;

            canMove = false;
            menu.DOLocalMoveY(0, duration)
                .OnComplete(() => canMove = true);
        }
    }
}