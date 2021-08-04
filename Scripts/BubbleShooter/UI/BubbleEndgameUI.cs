using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace BubbleShooter.UI
{
    public class BubbleEndgameUI : MonoBehaviour
    {
        [SerializeField] GameObject root;

        private void Awake()
        {
            root.SetActive(false);
        }

        private IEnumerator Start()
        {
            root.SetActive(true);

            var rect = root.GetComponent<RectTransform>();
            rect.localScale = Vector3.zero;
            yield return new WaitForSeconds(2.5f);
            
            rect.DOScale(Vector3.one, .5f).SetEase(Ease.OutBack);
        }
    }
}