#if !SOAR_R3

using System;
using System.Collections.Generic;

namespace Soar.Events
{
    public partial class GameEvent
    {
        protected readonly List<IDisposable> Disposables = new();

        public virtual partial void Raise()
        {
            foreach (var disposable in Disposables)
            {
                if (disposable is not Subscription subscription) continue;
                subscription.Invoke();
            }
        }

        public partial IDisposable Subscribe(Action action)
        {
            return Subscribe(action, withBuffer: false);
        }

        public IDisposable Subscribe(Action action, bool withBuffer)
        {
            var subscription = new Subscription(action, Disposables);
            
            if (Disposables.Contains(subscription))
            {
                return subscription;
            }
            
            Disposables.Add(subscription);
                
            if (withBuffer)
            {
                subscription.Invoke();
            }

            return subscription;
        }

        public override void Dispose()
        {
            Disposables.Dispose();
        }
    }

    public abstract partial class GameEvent<T>
    {
        public virtual partial void Raise(T param)
        {
            value = param;
            
            base.Raise();
            
            foreach (var disposable in Disposables)
            {
                if (disposable is not Subscription<T> subscription) continue;
                subscription.Invoke(value);
            }
        }

        public partial IDisposable Subscribe(Action<T> action)
        {
            return Subscribe(action, withBuffer: false);
        }

        public IDisposable Subscribe(Action<T> action, bool withBuffer)
        {
            var subscription = new Subscription<T>(action, Disposables);
            
            if (Disposables.Contains(subscription))
            {
                return subscription;
            }

            Disposables.Add(subscription);

            if (withBuffer)
            {
                subscription.Invoke(value);
            }

            return subscription;
        }
    }
}

#endif
