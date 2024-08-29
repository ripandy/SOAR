using System;
using UnityEngine;

namespace Soar.Events
{
    /// <summary>
    /// Core Game Event System which implements simple Pub-Sub pattern.
    /// </summary>
    [CreateAssetMenu(fileName = "GameEvent", menuName = MenuHelper.DefaultGameEventMenu + "GameEvent")]
    public partial class GameEvent : SoarCore
    {
        /// <summary>
        /// Publish the event to all subscribers.
        /// </summary>
        public virtual partial void Raise();
        
        /// <summary>
        /// Subscribe to the event and Invoke an Action upon event raise.
        /// </summary>
        /// <param name="action">Action to be invoked upon event raise.</param>
        /// <returns>Disposable for when subscription is no longer necessary.</returns>
        public partial IDisposable Subscribe(Action action);
    }

    /// <summary>
    /// Generic base class for event system with parameter.
    /// </summary>
    /// <typeparam name="T">Parameter type for the event system</typeparam>
    public abstract partial class GameEvent<T> : GameEvent
    {
        [SerializeField] protected T value;

        internal Type Type => typeof(T);

        public override void Raise()
        {
            Raise(value);
        }

        internal virtual void ResetInternal()
        {
            value = default;
        }

        internal override void OnQuit()
        {
            ResetInternal();
            base.OnQuit();
        }
        
        /// <summary>
        /// Publish value to all subscribers.
        /// </summary>
        /// /// <param name="valueToRaise">Value to be published.</param>
        public virtual partial void Raise(T valueToRaise);
        
        /// <summary>
        /// Subscribe to the event and processes published value on the provided Action.
        /// </summary>
        /// <param name="action">Action to be invoked upon event raise.</param>
        /// <returns>Disposable for when subscription is no longer necessary.</returns>
        public partial IDisposable Subscribe(Action<T> action);
    }
}
