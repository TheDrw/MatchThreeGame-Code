using UnityEngine;
using UnityEngine.EventSystems;
using MatchThree.Audio;

namespace MatchThree.UI.Effects
{
    public class SelectorEnterAudio : MonoBehaviour, IPointerEnterHandler
    {
        public void OnPointerEnter(PointerEventData eventData)
        {
            MenuSFX.Play?.ButtonSelected();
        }
    }
}