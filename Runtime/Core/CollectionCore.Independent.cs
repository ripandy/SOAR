#if !SOAR_R3

using System;
using System.Collections.Generic;
using System.Linq;

namespace Soar.Collections
{
    // List
    public abstract partial class Collection<T>
    {
        private readonly List<IDisposable> onAddSubscriptions = new();
        private readonly List<IDisposable> onRemoveSubscriptions = new();
        private readonly List<IDisposable> onClearSubscriptions = new();
        private readonly List<IDisposable> countSubscriptions = new();
        private readonly List<IDisposable> valueSubscriptions = new();
        
        public partial IDisposable SubscribeOnAdd(Action<T> action) => SubscribeOnAdd(action, withBuffer: false);
        public partial IDisposable SubscribeOnRemove(Action<T> action) => SubscribeOnRemove(action, withBuffer: false);
        public partial IDisposable SubscribeOnClear(Action action) => SubscribeOnClear(action, withBuffer: false);
        public partial IDisposable SubscribeToCount(Action<int> action) => SubscribeToCount(action, withBuffer: false);
        public partial IDisposable SubscribeToValues(Action<int, T> action) => SubscribeToValues(action, withBuffer: false);

        private (int index, T value) lastUpdated;
        
        public IDisposable SubscribeOnAdd(Action<T> action, bool withBuffer)
        {
            var subscription = new Subscription<T>(action, onAddSubscriptions);
            
            onAddSubscriptions.Add(subscription);
            
            if (withBuffer && list.Count > 0)
            {
                subscription.Invoke(list.Last());
            }

            return subscription;
        }

        public IDisposable SubscribeOnRemove(Action<T> action, bool withBuffer)
        {
            var subscription = new Subscription<T>(action, onRemoveSubscriptions);
            
            onRemoveSubscriptions.Add(subscription);
            
            if (withBuffer)
            {
                subscription.Invoke(lastRemoved);
            }

            return subscription;
        }

        public IDisposable SubscribeOnClear(Action action, bool withBuffer)
        {
            var subscription = new Subscription(action, onClearSubscriptions);

            onClearSubscriptions.Add(subscription);
            
            if (withBuffer)
            {
                subscription.Invoke();
            }

            return subscription;
        }
        
        public IDisposable SubscribeToCount(Action<int> action, bool withBuffer)
        {
            var subscription = new Subscription<int>(action, countSubscriptions);
            
            countSubscriptions.Add(subscription);
            
            if (withBuffer)
            {
                subscription.Invoke(Count);
            }

            return subscription;
        }

        public IDisposable SubscribeToValues(Action<int, T> action, bool withBuffer)
        {
            var subscription = new IndexValueSubscription<T>(action, valueSubscriptions);
            
            valueSubscriptions.Add(subscription);
            
            if (withBuffer)
            {
                subscription.Invoke(lastUpdated.index, lastUpdated.value);
            }

            return subscription;
        }
        
        private partial void RaiseOnAdd(T addedValue)
        {
            foreach (var disposable in onAddSubscriptions)
            {
                if (disposable is Subscription<T> valueSubscription)
                {
                    valueSubscription.Invoke(addedValue);
                }
            }
        }
        
        private partial void RaiseOnRemove(T removedValue)
        {
            foreach (var disposable in onRemoveSubscriptions)
            {
                if (disposable is Subscription<T> valueSubscription)
                {
                    valueSubscription.Invoke(removedValue);
                }
            }
        }
        
        private partial void RaiseOnClear()
        {
            foreach (var disposable in onClearSubscriptions)
            {
                if (disposable is Subscription subscription)
                {
                    subscription.Invoke();
                }
            }
        }
        
        private partial void RaiseCount()
        {
            foreach (var disposable in countSubscriptions)
            {
                if (disposable is Subscription<int> countSubscription)
                {
                    countSubscription.Invoke(Count);
                }
            }
        }

        private partial void RaiseValueAt(int index, T value)
        {
            if (valueEventType == ValueEventType.OnChange && list[index].Equals(value)) return;

            foreach (var disposable in valueSubscriptions)
            {
                if (disposable is IndexValueSubscription<T> valueSubscription)
                {
                    valueSubscription.Invoke(index, value);
                }
            }
            
            lastUpdated = (index, value);
        }
        
        private partial void ClearValueSubscriptions()
        {
            foreach (var subscription in valueSubscriptions)
            {
                subscription.Dispose();
            }
            valueSubscriptions.Clear();
        }
        
        private partial void DisposeSubscriptions()
        {
            onAddSubscriptions.Dispose();
            onRemoveSubscriptions.Dispose();
            onClearSubscriptions.Dispose();
            countSubscriptions.Dispose();
            ClearValueSubscriptions();
        }
    }
    
    // Dictionary
    public abstract partial class Collection<TKey, TValue>
    {
        private readonly List<IDisposable> valueSubscriptions = new();
        
        private (TKey Key, TValue Value) lastUpdated;

        public IDisposable SubscribeOnAdd(Action<TKey, TValue> action, bool withBuffer)
        {
            return base.SubscribeOnAdd(pair => action.Invoke(pair.Key, pair.Value), withBuffer);
        }

        public IDisposable SubscribeOnAdd(Action<TKey, TValue> action)
        {
            return SubscribeOnAdd(action, withBuffer: false);
        }

        public IDisposable SubscribeOnRemove(Action<TKey, TValue> action, bool withBuffer)
        {
            return base.SubscribeOnRemove(pair => action.Invoke(pair.Key, pair.Value), withBuffer);
        }

        public IDisposable SubscribeOnRemove(Action<TKey, TValue> action)
        {
            return SubscribeOnRemove(action, withBuffer: false);
        }

        public partial IDisposable SubscribeToValues(Action<TKey, TValue> action)
        {
            return SubscribeToValues(action, withBuffer: false);
        }

        public IDisposable SubscribeToValues(Action<TKey, TValue> action, bool withBuffer)
        {
            var subscription = new KeyValueSubscription<TKey, TValue>(action, valueSubscriptions);
            
            valueSubscriptions.Add(subscription);
            
            if (withBuffer)
            {
                subscription.Invoke(lastUpdated.Key, lastUpdated.Value);
            }

            return subscription;
        }
        
        private partial void RaiseValue(TKey key, TValue value)
        {
            if (valueEventType == ValueEventType.OnChange && IsValueEqual()) return;

            foreach (var disposable in valueSubscriptions)
            {
                if (disposable is not KeyValueSubscription<TKey, TValue> valueSubscription) continue;
                valueSubscription.Invoke(key, value);
            }

            bool IsValueEqual()
            {
                return dictionary.TryGetValue(key, out var val) && val.Equals(value);
            }
        }

        private partial void ClearValueSubscriptions()
        {
            foreach (var subscription in valueSubscriptions)
            {
                subscription.Dispose();
            }
            valueSubscriptions.Clear();
        }
    }
}

#endif