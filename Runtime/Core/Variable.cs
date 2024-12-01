using System;
using Soar.Events;
using UnityEngine;

namespace Soar.Variables
{
    /// <summary>
    /// Core Variable System which extends Game Event System to handle value related events.
    /// Variable System is Game Event with Value property accessible at non-event context.
    /// </summary>
    /// <typeparam name="T">Type to be used as Value. Struct and Primitive types is encouraged.</typeparam>
    public abstract partial class Variable<T> : GameEvent<T>
    {
        [Tooltip("Set how value event behave.\nValue Assign: Raise when value is assigned regardless of value.\nValue Changed: Raise only when value is changed.")]
        [SerializeField] protected ValueEventType valueEventType;

        [Tooltip("If true will reset value when play mode end. Otherwise, keep runtime value. Due to shallow copying of class types, it is better avoid using autoResetValue on Class type.")]
        [SerializeField] protected bool autoResetValue;
        
        public virtual T Value
        {
            get => value;
            set => Raise(value);
        }

        private bool IsValueEquals(T valueToCompare)
        {
            return value == null && valueToCompare == null ||
                   value != null && valueToCompare != null && value.Equals(valueToCompare);
        }
        
        internal Type Type => typeof(T);
        
        private T initialValue;
        
        // MEMO: Hack to handle deep copy of class type.
        //       Better to avoid class type on Variable.
        private string initialValueJsonString;
        
        /// <summary>
        /// Value stored at initialization time.
        /// </summary>
        public T InitialValue
        {
            get
            {
                if (Type.IsSimpleType()) return initialValue;
                
                try
                {
                    initialValue = JsonUtility.FromJson<T>(initialValueJsonString);
                }
                catch (ArgumentException)
                {
                    // MEMO: Engine types like Transform, GameObject, etc., cannot be handled using Json.
                    //       Let engine types goes through try-catch block.
                    // TODO: Actually check for engine types or "Fix" the hack.
                }
                return initialValue;
            }
            protected set
            {
                if (Type.IsSimpleType())
                {
                    initialValue = value;
                    return;
                }
                
                try
                {
                    initialValueJsonString = JsonUtility.ToJson(value);
                }
                catch (ArgumentException)
                {
                    // MEMO: Engine types like Transform, GameObject, etc., cannot be handled using Json.
                    //       Let engine types goes through try-catch block.
                    // TODO: Actually check for engine types or "Fix" the hack.
                }
            }
        }

        /// <summary>
        /// Reset to value at initialization time.
        /// </summary>
        public void ResetValue()
        {
            Value = InitialValue;
        }

        internal override void ResetInternal()
        {
            if (!autoResetValue) return;
            ResetValue();
        }

        public static implicit operator T(Variable<T> variable) => variable.Value;

        public override string ToString() => Value.ToString();

        internal override void Initialize()
        {
            InitialValue = Value;
            base.Initialize();
        }
        
        // List of Partial methods. Implemented in each respective integrated Library.
        public override partial void Raise(T valueToRaise);
        
        /// <summary>
        /// Subscribe to the event and processes published value along with previous value on the provided Action.
        /// </summary>
        /// <param name="action">Action to be invoked upon event raise.</param>
        /// <returns>Disposable for when subscription is no longer necessary.</returns>
        public partial IDisposable Subscribe(Action<T, T> action);
        
        /// <summary>
        /// Subscribe to the event and processes published value as PairwiseValue which holds oldValue along with newValue.
        /// </summary>
        /// <param name="action">Action to be invoked upon event raise.</param>
        /// <returns>Disposable for when subscription is no longer necessary.</returns>
        public partial IDisposable Subscribe(Action<PairwiseValue<T>> action);
    }
}