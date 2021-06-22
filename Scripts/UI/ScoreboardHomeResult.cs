using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchThree.Data;
using UnityEngine.UI;

namespace MatchThree.UI
{
    public abstract class ScoreboardHomeResult : MonoBehaviour
    {
        public virtual void InitializeTextResult(FinishedGameResult res, bool enableHighlight)
        {
            if(enableHighlight)
            {
                GetComponent<Image>().enabled = true;
            }
        }
    }
}