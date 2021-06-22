using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchThree.Core;

namespace MatchThree.Data
{
    [CreateAssetMenu(menuName = "Board/Game Pieces Shape")]
    public class GamePieceShapes : ScriptableObject
    {
        [Space(10)]
        [SerializeField] int currentShapeIndex = 0;
        [SerializeField] Sprite[] shapes;

        public Sprite CurrentShape => shapes[currentShapeIndex];

        private void OnValidate()
        {
            currentShapeIndex = Mathf.Clamp(currentShapeIndex, 0, shapes.Length-1);
        }

        public Sprite NextShape()
        {
            currentShapeIndex = (currentShapeIndex + 1) % shapes.Length;

            return shapes[currentShapeIndex];
        }

        public Sprite PreviousShape()
        {
            currentShapeIndex--;
            if(currentShapeIndex < 0)
            {
                currentShapeIndex = shapes.Length - 1;
            }

            return shapes[currentShapeIndex];
        }
    }
}