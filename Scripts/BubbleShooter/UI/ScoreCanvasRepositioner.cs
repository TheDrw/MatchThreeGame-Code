using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BubbleShooter.Core;

namespace BubbleShooter.UI
{
    public class ScoreCanvasRepositioner : MonoBehaviour
    {
        [SerializeField] BubbleShooterBoard board;

        private void Start()
        {
            transform.position = transform.position +  Vector3.up * board.Height + Vector3.up * 0.1f;
        }
    }
}