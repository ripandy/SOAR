#if SOAR_R3

using System;
using R3;

namespace Soar.Variables
{
    public abstract partial class VariableCore<T>
    {
        public override partial void Raise(T valueToRaise)
        {
            oldValue = value;
            
            if (valueEventType == ValueEventType.OnChange && IsValueEquals(valueToRaise)) return;
            
            base.Raise(valueToRaise);
        }
        
        public partial IDisposable Subscribe(Action<T, T> action)
        {
            return ValueSubject.Subscribe(newValue => action.Invoke(oldValue, newValue));
        }
    }
}

#endif