using MatchThree.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter.Core
{
    public class Border : GamePiece
    {
        protected override void Start()
        {
            // not gonna lie. kind of a hack to prevent the 
            // base class from calling start to change the sprite
        }
    }
}