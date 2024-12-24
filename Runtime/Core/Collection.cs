using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Soar.Collections
{
    public abstract partial class Collection<T> : SoarCore, ICollection<T>, IReadOnlyCollection<T>, ICollection
    {
        [SerializeField] protected System.Collections.Generic.List<T> list = new();
        
        [Tooltip("Set how value event behave.\nValue Assign: Raise when value is assigned regardless of value.\nValue Changed: Raise only when value is changed.")]
        [SerializeField] protected ValueEventType valueEventType;
        
        [Tooltip("If true will reset value(s) when play mode end. Otherwise, keep runtime value. Due to shallow copying of class types, it is better avoid using autoResetValue on Class type.")]
        [SerializeField] protected bool autoResetValue;
        
        private readonly System.Collections.Generic.List<T> initialValue = new();
        internal T lastRemoved;
        
        public bool IsSynchronized => (list as ICollection).IsSynchronized;
        public virtual object SyncRoot => syncRoot;
        private readonly object syncRoot = new();
        
        private System.Collections.Generic.List<T> InitialValue
        {
            get
            {
                lock (syncRoot)
                {
                    return initialValue;
                }
            }
            set
            {
                lock (syncRoot)
                {
                    initialValue.Clear();
                    if (value == null) return;
                    initialValue.AddRange(value);
                }
            }
        }
        
        public int Count
        {
            get
            {
                lock (syncRoot)
                {
                    return list.Count;
                }
            }
        }
        
        public bool IsReadOnly => false;
        
        public void Add(T item)
        {
            AddInternal(item);
        }
        
        internal virtual void AddInternal(T item)
        {
            lock (syncRoot)
            {
                var index = list.Count;
                list.Add(item);
                RaiseOnAdd(item);
                RaiseValueAt(index, item);
                RaiseCount();
            }
        }
        
        public void AddRange(IEnumerable<T> items)
        {
            AddRangeInternal(items.ToArray());
        }
        
        public void AddRange(T[] items)
        {
            AddRangeInternal(items);
        }
        
        internal virtual void AddRangeInternal(T[] items)
        {
            lock (syncRoot)
            {
                foreach (var item in items)
                {
                    var index = list.Count;
                    list.Add(item);
                    RaiseOnAdd(item);
                    RaiseValueAt(index, item);
                }
                RaiseCount();
            }
        }
        
        public void Clear()
        {
            ClearInternal();
        }
        
        internal virtual void ClearInternal()
        {
            lock (syncRoot)
            {
                list.Clear();
                RaiseOnClear();
                RaiseCount();
            }
        }
        
        public bool Contains(T item)
        {
            lock (syncRoot)
            {
                return list.Contains(item);
            }
        }
        
        public void Copy(IEnumerable<T> others)
        {
            CopyInternal(others);
        }
        
        internal virtual void CopyInternal(IEnumerable<T> others)
        {
            lock (syncRoot)
            {
                list.Clear();
                list.AddRange(others);
            }
        }
        
        public void CopyTo(Array array, int index)
        {
            lock (syncRoot)
            {
                (list as ICollection).CopyTo(array, index);
            }
        }
        
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (syncRoot)
            {
                list.CopyTo(array, arrayIndex);
            }
        }
        
        public void ForEach(Action<T> action)
        {
            lock (syncRoot)
            {
                foreach (var item in list)
                {
                    action(item);
                }
            }
        }
        
        public IEnumerator<T> GetEnumerator()
        {
            lock (syncRoot)
            {
                foreach (var item in list)
                {
                    yield return item;
                }
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public bool Remove(T item)
        {
            return RemoveInternal(item);
        }
        
        internal virtual bool RemoveInternal(T item)
        {
            lock (syncRoot)
            {
                var index = list.IndexOf(item);
                if (index < 0) return false;
                
                lastRemoved = list[index];
                list.RemoveAt(index);
                RaiseOnRemove(lastRemoved);
                RaiseCount();
                return true;
            }
        }
        
        internal override void Initialize()
        {
            InitialValue = list;
            base.Initialize();
        }
        
        /// <summary>
        /// Reset value(s) to InitialValue
        /// </summary>
        public void ResetValues() => CopyInternal(InitialValue);
        
        private void ResetInternal()
        {
            if (!autoResetValue) return;
            ResetValues();
        }
        
        internal override void OnQuit()
        {
            ResetInternal();
            base.OnQuit();
        }
        
        // List of Partial methods. Implemented in each respective integrated Library.
        internal partial void RaiseOnAdd(T addedValue);
        internal partial void RaiseOnRemove(T removedValue);
        internal partial void RaiseCount();
        internal partial void RaiseValueAt(int index, T value);
        private partial void RaiseOnClear();
        
        /// <summary>
        /// Subscribe to OnAdd event. Will be called when new value is added and is called with the added value.
        /// </summary>
        /// <param name="action">Action to be executed on event call.</param>
        /// <returns>Subscription's IDisposable. Call Dispose() to Unsubscribe.</returns>
        public partial IDisposable SubscribeOnAdd(Action<T> action);
        
        /// <summary>
        /// Subscribe to OnRemove event. Will be called when value is removed and is called with the removed value.
        /// </summary>
        /// <param name="action">Action to be executed on event call.</param>
        /// <returns>Subscription's IDisposable. Call Dispose() to Unsubscribe.</returns>
        public partial IDisposable SubscribeOnRemove(Action<T> action);
        
        /// <summary>
        /// Subscribe to Count values. Will be called when Count value is changed and is called with the new Count value.
        /// </summary>
        /// <param name="action">Action to be executed on event call.</param>
        /// <returns>Subscription's IDisposable. Call Dispose() to Unsubscribe.</returns>
        public partial IDisposable SubscribeToCount(Action<int> action);
        
        /// <summary>
        /// Subscribe to OnClear event. Will be called when Clear() is called without any arguments.
        /// </summary>
        /// <param name="action">Action to be executed on event call.</param>
        /// <returns>Subscription's IDisposable. Call Dispose() to Unsubscribe.</returns>
        public partial IDisposable SubscribeOnClear(Action action);
        
        /// <summary>
        /// Subscribe to value update event. Will be called when element value is updated and is called with element index and value.
        /// </summary>
        /// <param name="action">Action to be executed on event call.</param>
        /// <returns>Subscription's IDisposable. Call Dispose() to Unsubscribe.</returns>
        public partial IDisposable SubscribeToValues(Action<int, T> action);
        
        /// <summary>
        /// Subscribe to value update event. Will be called when element value is updated and is called with IndexValuePair data structure.
        /// IndexValuePair contains index and value of the element updated.
        /// </summary>
        /// <param name="action">Action to be executed on event call.</param>
        /// <returns>Subscription's IDisposable. Call Dispose() to Unsubscribe.</returns>
        public partial IDisposable SubscribeToValues(Action<IndexValuePair<T>> action);
    }
}