using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchThree.Effects
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteAlphaPacing : MonoBehaviour
    {
        SpriteRenderer spriteRenderer;
        Color alphaColor;
        float startTime = 0f;

        private void OnEnable()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            alphaColor = spriteRenderer.color;
        }

        // Update is called once per frame
        void Update()
        {
            alphaColor.a = 0.25f * Mathf.Sin(3f * (startTime - 1.55f)) + 0.75f; // for visual reasoning https://www.geogebra.org/m/F2DbmqCB
            startTime += Time.deltaTime;

            spriteRenderer.color = alphaColor;
        }
    }
}