#if !SOAR_R3

using System;
using System.Collections.Generic;

namespace Soar.Events
{
    public partial class GameEvent
    {
        protected readonly List<IDisposable> subscriptions = new();

        public virtual partial void Raise()
        {
            foreach (var disposable in subscriptions)
            {
                if (disposable is not Subscription subscription) continue;
                subscription.Invoke();
            }
        }

        public partial IDisposable Subscribe(Action action)
        {
            var subscription = new Subscription(action, subscriptions);
            subscriptions.Add(subscription);
            return subscription;
        }

        public override void Dispose()
        {
            subscriptions.Dispose();
        }
    }

    public abstract partial class GameEvent<T>
    {
        public virtual partial void Raise(T valueToRaise)
        {
            value = valueToRaise;
            base.Raise();
            
            foreach (var disposable in subscriptions)
            {
                if (disposable is not Subscription<T> subscription) continue;
                subscription.Invoke(value);
            }
        }

        public partial IDisposable Subscribe(Action<T> action)
        {
            var subscription = new Subscription<T>(action, subscriptions);
            subscriptions.Add(subscription);
            return subscription;
        }
    }
}

#endif
