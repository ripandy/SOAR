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
    public abstract class VariableCore<T> : GameEvent<T>
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

        public override void Raise(T valueToRaise)
        {
            if (valueEventType == ValueEventType.OnChange && IsValueEquals(valueToRaise)) return;
            base.Raise(valueToRaise);
        }

        private bool IsValueEquals(T valueToCompare)
        {
            return value == null && valueToCompare == null ||
                   value != null && valueToCompare != null && value.Equals(valueToCompare);
        }

        /// <summary>
        /// Value stored at initialization time.
        /// </summary>
        private T initialValue;
        
        // MEMO: Hack to handle shallow copy of class type.
        //       Better to avoid class type on Variable.
        private string initialValueJsonString;

        public T InitialValue
        {
            get
            {
                if (!Type.IsSimpleType())
                {
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
                }
                return initialValue;
            }
            protected set
            {
                if (!Type.IsSimpleType())
                {
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
                initialValue = value;
            }
        }

        /// <summary>
        /// Reset to value at initialization time.
        /// </summary>
        public void ResetValue() => Value = InitialValue;

        protected override void ResetInternal()
        {
            if (!autoResetValue)
            {
                return;
            }
            
            ResetValue();
        }

        public static implicit operator T(VariableCore<T> variable) => variable.Value;

        public override string ToString() => Value.ToString();

        protected override void Initialize()
        {
            InitialValue = Value;
            base.Initialize();
        }
    }
}