using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchThree.Effects
{
    [CreateAssetMenu(menuName = "Board/Visual Effects")]
    public class BoardVFX : ScriptableObject
    {
        [SerializeField] GameObject vanish = null; // temp
        [SerializeField] GameObject clearBoard = null;

        public GameObject Vanish => vanish;
        public GameObject ClearBoard => clearBoard;
    }
}