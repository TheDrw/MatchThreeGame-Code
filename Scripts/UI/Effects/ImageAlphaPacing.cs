using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MatchThree.UI.Effects
{
    [RequireComponent(typeof(Image))]
    public class ImageAlphaPacing : MonoBehaviour
    {
        [SerializeField] float speed = 1;

        Image img;
        Color alphaColor;
        float startTime = 0f;

        private void OnEnable()
        {
            img = GetComponent<Image>();
            alphaColor = img.color;
        }

        // Update is called once per frame
        void Update()
        {
            alphaColor.a = 0.25f * Mathf.Sin(3f * (startTime - 1.55f)) + 0.75f; // for visual reasoning https://www.geogebra.org/m/F2DbmqCB
            startTime += Time.deltaTime * speed;

            img.color = alphaColor;
        }
    }
}