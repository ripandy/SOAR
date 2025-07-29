using System;
using TMPro;
using UnityEngine;

namespace Soar.Events.Sample
{
    public class EventSubscriber : MonoBehaviour
    {
        [SerializeField] private GameEvent gameEvent;
        [SerializeField] private TMP_Text displayText;
        [SerializeField] private string[] textsToDisplay =
        {
            "Every", "time", "event", "is", "fired", "a", "word", "is", "added", "to", "make", "a", "full", "sentence."
        };

        private int index;
        private IDisposable subscription;

        private void Start()
        {
            subscription = gameEvent.Subscribe(OnEventRaised);
        }

        private void OnEventRaised()
        {
            if (index >= textsToDisplay.Length)
            {
                displayText.text = "";
                index = 0;
            }
            
            displayText.text += textsToDisplay[index] + " ";
            index++;
        }

        private void OnDestroy()
        {
            subscription.Dispose();
        }
    }
}