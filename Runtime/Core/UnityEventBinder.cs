using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Soar.Events
{
    public class UnityEventBinder : MonoBehaviour
    {
        [SerializeField] protected GameEvent gameEventToListen;
        [Space, SerializeField] private UnityEvent onGameEventRaised;

        protected readonly List<IDisposable> Subscriptions = new();

        protected virtual void Start()
        {
            Subscriptions.Add(gameEventToListen.Subscribe(onGameEventRaised.Invoke));
        }

        protected virtual void OnDestroy()
        {
            foreach (var subscription in Subscriptions)
            {
                subscription.Dispose();
            }
        }
    }

    public abstract class UnityEventBinder<T> : UnityEventBinder
    {
        [SerializeField] protected TypedUnityEvent onTypedGameEventRaised;
        
        protected override void Start()
        {
            base.Start();
            if (gameEventToListen is GameEvent<T> typedEvent)
            {
                Subscriptions.Add(typedEvent.Subscribe(onTypedGameEventRaised.Invoke));
            }
        }

        [Serializable]
        protected class TypedUnityEvent : UnityEvent<T> { }
    }
}