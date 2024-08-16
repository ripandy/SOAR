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
        private readonly Dictionary<int, List<IDisposable>> valueSubscriptions = new();
        private readonly List<IDisposable> countSubscriptions = new();
        
        public partial IDisposable SubscribeOnAdd(Action<T> action) => SubscribeOnAdd(action, withBuffer: false);
        public partial IDisposable SubscribeOnRemove(Action<T> action) => SubscribeOnRemove(action, withBuffer: false);
        public partial IDisposable SubscribeOnClear(Action action) => SubscribeOnClear(action, withBuffer: false);
        public partial IDisposable SubscribeToCount(Action<int> action) => SubscribeToCount(action, withBuffer: false);
        public partial IDisposable SubscribeToValueAt(int index, Action<T> action) => SubscribeToValueAt(index, action, withBuffer: false);

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

        public IDisposable SubscribeToValueAt(int index, Action<T> action, bool withBuffer)
        {
            if (!valueSubscriptions.TryGetValue(index, out var subscriptions))
            {
                subscriptions = new List<IDisposable>();
                valueSubscriptions.Add(index, subscriptions);
            }
            
            var subscription = new Subscription<T>(action, subscriptions);
            
            subscriptions.Add(subscription);
            
            if (withBuffer && index < list.Count && list[index] != null)
            {
                subscription.Invoke(list[index]);
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
            if (!valueSubscriptions.TryGetValue(index, out var subscriptions)) return;

            foreach (var disposable in subscriptions)
            {
                if (disposable is Subscription<T> valueSubscription)
                {
                    valueSubscription.Invoke(value);
                }
            }
        }
        
        private partial void IncrementValueSubscriptions(int index)
        {
            for (var i = list.Count; i > index; i--)
            {
                valueSubscriptions.TryChangeKey(i - 1, i);
            }
        }

        private partial void SwitchValueSubscription(int oldIndex, int newIndex)
        {
            valueSubscriptions.TryChangeKey(oldIndex, newIndex);
        }
        
        private partial void ClearValueSubscriptions()
        {
            foreach (var subscriptions in valueSubscriptions.Values)
            {
                for (var i = subscriptions.Count - 1; i >= 0; i--)
                {
                    subscriptions[i].Dispose();
                }
                subscriptions.Clear();
            }
            valueSubscriptions.Clear();
        }
        
        private partial void RemoveValueSubscription(int index)
        {
            if (!valueSubscriptions.TryGetValue(index, out var subscriptions)) return;
            subscriptions.Dispose();
            valueSubscriptions.Remove(index);
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
        private readonly IDictionary<TKey, IList<IDisposable>> valueSubscriptions = new Dictionary<TKey, IList<IDisposable>>();

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

        public partial IDisposable SubscribeToValue(TKey key, Action<TValue> action)
        {
            return SubscribeToValue(key, action, withBuffer: false);
        }

        public IDisposable SubscribeToValue(TKey key, Action<TValue> action, bool withBuffer)
        {
            if (!valueSubscriptions.TryGetValue(key, out var subscriptions))
            {
                subscriptions = new List<IDisposable>();
                valueSubscriptions.Add(key, subscriptions);
            }
            
            var subscription = new Subscription<TValue>(action, subscriptions);
            
            subscriptions.Add(subscription);
            
            if (withBuffer && dictionary.TryGetValue(key, out var value) && value != null)
            {
                subscription.Invoke(value);
            }

            return subscription;
        }
        
        private partial void RaiseValue(TKey key, TValue value)
        {
            if (valueEventType == ValueEventType.OnChange && IsValueEqual()) return;
            if (!valueSubscriptions.TryGetValue(key, out var subscriptions)) return;

            foreach (var disposable in subscriptions)
            {
                if (disposable is not Subscription<TValue> valueSubscription) continue;
                valueSubscription.Invoke(value);
            }

            bool IsValueEqual()
            {
                return dictionary.TryGetValue(key, out var val) && val.Equals(value);
            }
        }

        private partial void ClearValueSubscriptions()
        {
            foreach (var subscriptions in valueSubscriptions.Values)
            {
                foreach (var disposable in subscriptions)
                {
                    disposable.Dispose();
                }
                subscriptions.Clear();
            }
            valueSubscriptions.Clear();
        }

        private partial void RemoveValueSubscription(TKey key)
        {
            if (!valueSubscriptions.TryGetValue(key, out var subscriptions)) return;
            subscriptions.Dispose();
            valueSubscriptions.Remove(key);
        }
    }
}

#endif