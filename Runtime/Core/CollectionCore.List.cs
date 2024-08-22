using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Soar.Collections
{
    public abstract partial class Collection<T> : SoarCore, IList<T>, IReadOnlyList<T>
    {
        [SerializeField] protected List<T> list = new();
        
        [Tooltip("Set how value event behave.\nValue Assign: Raise when value is assigned regardless of value.\nValue Changed: Raise only when value is changed.")]
        [SerializeField] protected ValueEventType valueEventType;
        
        [Tooltip("If true will reset value(s) when play mode end. Otherwise, keep runtime value. Due to shallow copying of class types, it is better avoid using autoResetValue on Class type.")]
        [SerializeField] protected bool autoResetValue;
        
        private readonly List<T> initialValue = new();
        private readonly object syncRoot = new();
        private T lastRemoved;
        
        public T this[int index]
        {
            get
            {
                lock (syncRoot)
                {
                    return list[index];
                }
            }
            set
            {
                lock (syncRoot)
                {
                    list[index] = value;
                    RaiseValueAt(index, value);
                }
            }
        }

        private List<T> InitialValue
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
            AddRange(items.ToArray());
        }

        public void AddRange(T[] items)
        {
            lock (syncRoot)
            {
                foreach (var item in items)
                {
                    var index = list.Count;
                    list.Add(item);
                    RaiseOnAdd(item);
                    RaiseValueAt(index, item);
                    RaiseCount();
                }
            }
        }
        
        public void Clear()
        {
            lock (syncRoot)
            {
                ClearValueSubscriptions();
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
            lock (syncRoot)
            {
                list.Clear();
                list.AddRange(others);
            }
        }
        
        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (syncRoot)
            {
                list.CopyTo(array, arrayIndex);
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
        
        public int IndexOf(T item)
        {
            lock (syncRoot)
            {
                return list.IndexOf(item);
            }
        }
        
        public void Insert(int index, T item)
        {
            lock (syncRoot)
            {
                list.Insert(index, item);
                RaiseOnAdd(item);
                RaiseValueAt(index, item);
                RaiseCount();
            }
        }
        
        public void InsertRange(int index, T[] items)
        {
            lock (syncRoot)
            {
                for (var i = 0; i < items.Length; i++)
                {
                    var idx = i + index;
                    var item = items[i];
                    list.Insert(idx, item);
                    RaiseOnAdd(item);
                    RaiseValueAt(idx, item);
                    RaiseCount();
                }
            }
        }
        
        public void InsertRange(int index, IEnumerable<T> items)
        {
            InsertRange(index, items.ToArray());
        }
        
        public bool Remove(T item)
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

        public void RemoveAt(int index)
        {
            lock (syncRoot)
            {
                lastRemoved = list[index];
                list.RemoveAt(index);
                RaiseOnRemove(lastRemoved);
                RaiseCount();
            }
        }

        public void Move(int oldIndex, int newIndex)
        {
            lock (syncRoot)
            {
                var removedItem = list[oldIndex];
                list.RemoveAt(oldIndex);
                list.Insert(newIndex, removedItem);
            }
        }
        
        protected override void Initialize()
        {
            InitialValue = list;
            base.Initialize();
        }
        
        /// <summary>
        /// Reset value(s) to InitialValue
        /// </summary>
        public void ResetValues() => Copy(InitialValue);
        
        private void ResetInternal()
        {
            if (!autoResetValue) return;
            ResetValues();
        }
        
        protected override void OnQuit()
        {
            ResetInternal();
            base.OnQuit();
        }

        public override void Dispose()
        {
            DisposeSubscriptions();
        }
        
        // List of Partial methods. Implemented in each respective integrated Library.
        private partial void RaiseOnAdd(T addedValue);
        private partial void RaiseOnRemove(T removedValue);
        private partial void RaiseCount();
        private partial void RaiseOnClear();
        private partial void RaiseValueAt(int index, T value);
        
        private partial void ClearValueSubscriptions();
        private partial void DisposeSubscriptions();
        
        // TODO: Summaries
        public partial IDisposable SubscribeOnAdd(Action<T> action);
        public partial IDisposable SubscribeOnRemove(Action<T> action);
        public partial IDisposable SubscribeOnClear(Action action);
        public partial IDisposable SubscribeToCount(Action<int> action);
        public partial IDisposable SubscribeToValues(Action<int, T> action);
    }
}