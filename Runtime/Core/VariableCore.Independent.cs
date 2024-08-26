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
                switch (disposable)
                {
                    case OldNewSubscription<T> subscription:
                        subscription.Invoke(oldValue, valueToRaise);
                        break;
                    case Subscription<PairwiseValue<T>> pairwiseSubscription:
                        pairwiseSubscription.Invoke(new PairwiseValue<T>(oldValue, valueToRaise));
                        break;
                }
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
        
        public partial IDisposable Subscribe(Action<PairwiseValue<T>> action)
        {
            return Subscribe(action, withBuffer: false);   
        }

        public IDisposable Subscribe(Action<PairwiseValue<T>> action, bool withBuffer)
        {
            var subscription = new Subscription<PairwiseValue<T>>(action, Disposables);
            
            Disposables.Add(subscription);

            if (withBuffer)
            {
                subscription.Invoke(new PairwiseValue<T>(oldValue, value));
            }

            return subscription;
        }
    }
}

#endif