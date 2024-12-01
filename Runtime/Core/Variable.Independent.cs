#if !SOAR_R3

using System;

namespace Soar.Variables
{
    public abstract partial class Variable<T>
    {
        public override partial void Raise(T valueToRaise)
        {
            var oldValue = value;
            
            if (valueEventType == ValueEventType.OnChange && IsValueEquals(valueToRaise)) return;
            
            base.Raise(valueToRaise);
            
            foreach (var disposable in subscriptions)
            {
                switch (disposable)
                {
                    case PairwiseSubscription<T> subscription:
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
            var subscription = new PairwiseSubscription<T>(action, subscriptions);
            subscriptions.Add(subscription);
            return subscription;
        }
        
        public partial IDisposable Subscribe(Action<PairwiseValue<T>> action)
        {
            var subscription = new Subscription<PairwiseValue<T>>(action, subscriptions);
            subscriptions.Add(subscription);
            return subscription;
        }
    }
}

#endif