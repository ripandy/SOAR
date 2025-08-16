using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Soar.Events
{
    [ExecuteAlways]
    public class UnityEventBinder : MonoBehaviour
    {
        [SerializeField] protected GameEvent gameEventToListen;
        [Space, SerializeField] private UnityEvent onGameEventRaised;

        protected readonly List<IDisposable> subscriptions = new();

        protected virtual void Start()
        {
            if (gameEventToListen == null)
            {
                Debug.LogWarning($"[{GetType().Name}]: No GameEvent assigned on {gameObject.name}.", this);
                return;
            }
            subscriptions.Add(gameEventToListen.Subscribe(onGameEventRaised.Invoke));
        }

        protected virtual void OnDestroy()
        {
            foreach (var subscription in subscriptions)
            {
                subscription.Dispose();
            }
            subscriptions.Clear();
        }
    }

    [ExecuteAlways]
    public abstract class UnityEventBinder<T> : UnityEventBinder
    {
        [SerializeField] protected TypedUnityEvent onTypedGameEventRaised;
        
        protected override void Start()
        {
            base.Start();
            if (gameEventToListen is not GameEvent<T> typedEvent) return;
            subscriptions.Add(typedEvent.Subscribe(onTypedGameEventRaised.Invoke));
        }

        [Serializable]
        protected class TypedUnityEvent : UnityEvent<T> { }
    }
}