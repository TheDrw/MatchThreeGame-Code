using UnityEngine;
using BubbleShooter.Core;
using TMPro;

namespace BubbleShooter.UI
{
    public class BubbleScoreUI : MonoBehaviour
    {
        [SerializeField] BubbleScore score;
        [SerializeField] TMP_Text scoreText;

        private void OnEnable()
        {
            score.OnScoreUpdate += UpdateScoreText;
        }

        private void OnDisable()
        {
            score.OnScoreUpdate -= UpdateScoreText;
        }

        void UpdateScoreText(int score)
        {
            scoreText.text = $"{score:000000}";
        }
    }
}