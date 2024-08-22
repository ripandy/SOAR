#if !SOAR_R3

using System;

namespace Soar.Variables
{
    public abstract partial class VariableCore<T>
    {
        public override partial void Raise(T valueToRaise)
        {
            oldValue = value;
            
            if (valueEventType == ValueEventType.OnChange && IsValueEquals(valueToRaise)) return;
            
            base.Raise(valueToRaise);
            
            foreach (var disposable in Disposables)
            {
                if (disposable is not OldNewSubscription<T> subscription) continue;
                subscription.Invoke(oldValue, valueToRaise);
            }
        }
        
        public partial IDisposable Subscribe(Action<T, T> action)
        {
            return Subscribe(action, withBuffer: false);   
        }

        public IDisposable Subscribe(Action<T, T> action, bool withBuffer)
        {
            var subscription = new OldNewSubscription<T>(action, Disposables);
            
            Disposables.Add(subscription);

            if (withBuffer)
            {
                subscription.Invoke(oldValue, value);
            }

            return subscription;
        }
    }
}

#endif