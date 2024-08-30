#if SOAR_R3

using System;
using R3;

namespace Soar.Variables
{
    public abstract partial class VariableCore<T>
    {
        private readonly Subject<PairwiseValue<T>> pairwiseValueSubject = new();
        
        public override partial void Raise(T valueToRaise)
        {
            oldValue = value;
            
            if (valueEventType == ValueEventType.OnChange && IsValueEquals(valueToRaise)) return;
            
            base.Raise(valueToRaise);
            
            pairwiseValueSubject.OnNext(new PairwiseValue<T>(oldValue, valueToRaise));
        }
        
        public partial IDisposable Subscribe(Action<T, T> action)
        {
            return ValueSubject.Subscribe(newValue => action.Invoke(oldValue, newValue));
        }
        
        // MEMO: R3 has its own Pairwise Observable Extension i.e. `AsObservable().Pairwise()`
        public partial IDisposable Subscribe(Action<PairwiseValue<T>> action)
        {
            return pairwiseValueSubject.Subscribe(action.Invoke);
        }
        
        public override void Dispose()
        {
            pairwiseValueSubject.Dispose();
            base.Dispose();
        }
    }
}

#endif