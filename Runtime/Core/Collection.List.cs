using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Soar.Collections
{
    public abstract partial class List<T> : Collection<T>, IList<T>, IReadOnlyList<T>, IList
    {
        public T this[int index]
        {
            get
            {
                lock (SyncRoot)
                {
                    return list[index];
                }
            }
            set
            {
                lock (SyncRoot)
                {
                    list[index] = value;
                    RaiseValueAt(index, value);
                }
            }
        }
        
        public int IndexOf(T item)
        {
            lock (SyncRoot)
            {
                return list.IndexOf(item);
            }
        }
        
        public void Insert(int index, T item)
        {
            lock (SyncRoot)
            {
                list.Insert(index, item);
                RaiseOnAdd(item);
                RaiseValueAt(index, item);
                RaiseCount();
            }
        }
        
        public void InsertRange(int index, T[] items)
        {
            lock (SyncRoot)
            {
                for (var i = 0; i < items.Length; i++)
                {
                    var idx = i + index;
                    var item = items[i];
                    list.Insert(idx, item);
                    RaiseOnAdd(item);
                    RaiseValueAt(idx, item);
                }
                RaiseCount();
            }
        }
        
        public void InsertRange(int index, IEnumerable<T> items)
        {
            InsertRange(index, items.ToArray());
        }
        
        public void Move(int oldIndex, int newIndex)
        {
            lock (SyncRoot)
            {
                var removedItem = list[oldIndex];
                list.RemoveAt(oldIndex);
                list.Insert(newIndex, removedItem);
                RaiseOnMove(removedItem, oldIndex, newIndex);
            }
        }
        
        public void RemoveAt(int index)
        {
            RemoveAtInternal(index);
        }
        
        internal virtual void RemoveAtInternal(int index)
        {
            lock (SyncRoot)
            {
                lastRemoved = list[index];
                list.RemoveAt(index);
                RaiseOnRemove(lastRemoved);
                RaiseCount();
            }
        }
        
        int IList.Add(object value)
        {
            if (value is not T tValue) return -1;
            Add(tValue);
            return Count;
        }

        bool IList.Contains(object value)
        {
            return value is T tValue && Contains(tValue);
        }

        int IList.IndexOf(object value)
        {
            return value is not T tValue ? -1 : IndexOf(tValue);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }

        void IList.Remove(object value)
        {
            Remove((T)value);
        }

        bool IList.IsFixedSize => false;
        
        object IList.this[int index]
        {
            get => this[index];
            set => this[index] = (T)value;
        }
        
        // List of Partial methods. Implemented in each respective integrated Library.
        private partial void RaiseOnMove(T value, int oldIndex, int newIndex);
        
        // TODO: Summaries on public methods
        public partial IDisposable SubscribeOnMove(Action<T, int, int> action);
        public partial IDisposable SubscribeOnMove(Action<MovedValueDto<T>> action);
    }
}