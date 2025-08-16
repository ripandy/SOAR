using UnityEngine;

namespace Soar.Events
{
    public sealed class BoolUnityEventBinder : UnityEventBinder<bool>
    {
        [SerializeField] private TypedUnityEvent onNegatedBoolEventRaised;

        protected override void Start()
        {
            base.Start();
            if (gameEventToListen is GameEvent<bool> typedEvent)
            {
                subscriptions.Add(typedEvent.Subscribe(value => onNegatedBoolEventRaised.Invoke(!value)));
            }
        }
    }
}