using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Soar.Events
{
    public class UnityEventBatchBinder : MonoBehaviour
    {
        [SerializeField] private GameEvent[] gameEventsToListen;
        [Space, SerializeField] private UnityEvent onGameEventRaised;

        private readonly List<IDisposable> subscriptions = new();

        private void Start()
        {
            foreach (var gameEventToListen in gameEventsToListen)
            {
                subscriptions.Add(gameEventToListen.Subscribe(onGameEventRaised.Invoke));   
            }
        }

        private void OnDestroy()
        {
            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            }
        }
    }
}