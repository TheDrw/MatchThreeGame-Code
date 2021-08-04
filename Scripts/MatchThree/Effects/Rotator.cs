using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchThree.Effects
{
    public class Rotator : MonoBehaviour
    {
        [Tooltip("If +, it goes counter clockwise. Else it goes clockwise.")]
        [SerializeField] float speed = .5f;

        private void Update()
        {
            transform.RotateAround(transform.position, transform.forward, speed);
        }
    }
}