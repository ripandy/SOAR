using Soar.Variables;
using System;
using TMPro;
using UnityEngine;

namespace Soar.Samples.ClickerGame
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private IntVariable scoreVariable;
        [SerializeField] private TMP_Text scoreText;

        private IDisposable subscription;

        private void OnEnable()
        {
            subscription = scoreVariable.Subscribe(UpdateScoreText);
            UpdateScoreText(scoreVariable.Value); // Update text with initial value
        }

        private void UpdateScoreText(int newScore)
        {
            if (scoreText == null) return;
            scoreText.text = $"Score: {newScore}";
        }

        private void OnDisable()
        {
            subscription?.Dispose();
        }
    }
}
