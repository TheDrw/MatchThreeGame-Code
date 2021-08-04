using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MatchThree.Data;

namespace MatchThree.UI
{
    public class GamePieceShapeSelectorUI : MonoBehaviour
    {
        [SerializeField] Image image;
        [SerializeField] GamePieceShapes shape;

        private void OnEnable()
        {
            image.sprite = shape.CurrentShape;
            StartCoroutine(ChangeColorAroundRoutine());
        }

        private IEnumerator ChangeColorAroundRoutine()
        {
            var waitOneSec = new WaitForSeconds(1f);
            while (true)
            {
                image.color = UnityEngine.Random.ColorHSV();
                yield return waitOneSec;
            }
        }

        public void Right()
        {
            image.sprite = shape.NextShape();
        }

        public void Left()
        {
            image.sprite = shape.PreviousShape();
        }

    }
}