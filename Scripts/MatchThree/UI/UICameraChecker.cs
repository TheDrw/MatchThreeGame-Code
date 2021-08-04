using UnityEngine;
using MatchThree.Cam;

namespace MatchThree.UI
{
    public class UICameraChecker : MonoBehaviour
    {
        void Start()
        {
            var canvas = GetComponent<Canvas>();
            if (canvas.worldCamera == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning($"WRN : World camera was not assigned on {this}. Will traverse through hierarchy to find it.", this);
#endif 

                canvas.worldCamera = FindObjectOfType<UICamera>().Cam;
            }
        }
    }
}