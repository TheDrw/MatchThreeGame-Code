using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MatchThree.Data;

namespace MatchThree.UI
{
    public abstract class PlacementResult : MonoBehaviour
    {
        [SerializeField] protected TMP_Text spot;
        public virtual void SetPlacementText(FinishedGameResult result, int spotVal)
        {
            spot.text = $"{spotVal}";
        }
    }
}