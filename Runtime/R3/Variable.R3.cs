#if SOAR_R3

using System;
using R3;

namespace Soar.Variables
{
    public abstract partial class Variable<T>
    {
        private Subject<PairwiseValue<T>> pairwiseValueSubject;
        private Subject<PairwiseValue<T>> PairwiseValueSubject
        {
            get
            {
                if (pairwiseValueSubject == null || pairwiseValueSubject.IsDisposed)
                {
                    pairwiseValueSubject = new Subject<PairwiseValue<T>>();
                }
                return pairwiseValueSubject;
            }
        }
        private T oldValue;
        
        public override partial void Raise(T valueToRaise)
        {
            oldValue = value;
            if (valueEventType == ValueEventType.OnChange && IsValueEquals(valueToRaise)) return;
            base.Raise(valueToRaise);
            PairwiseValueSubject.OnNext(new PairwiseValue<T>(oldValue, valueToRaise));
        }
        
        public partial IDisposable Subscribe(Action<T, T> action)
        {
            return ValueSubject.Subscribe(newValue => action.Invoke(oldValue, newValue));
        }
        
        // MEMO: R3 has its own Pairwise Observable Extension i.e. `AsObservable().Pairwise()`
        public partial IDisposable Subscribe(Action<PairwiseValue<T>> action)
        {
            return PairwiseValueSubject.Subscribe(action.Invoke);
        }
        
        public override void Dispose()
        {
            pairwiseValueSubject?.Dispose();
            base.Dispose();
        }
    }
}

#endif
