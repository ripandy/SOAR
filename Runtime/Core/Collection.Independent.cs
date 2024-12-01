#if !SOAR_R3

using System;
using System.Collections.Generic;

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

        public partial IDisposable SubscribeOnAdd(Action<T> action)
        {
            var subscription = new Subscription<T>(action, onAddSubscriptions);
            onAddSubscriptions.Add(subscription);
            return subscription;
        }

        public partial IDisposable SubscribeOnRemove(Action<T> action)
        {
            var subscription = new Subscription<T>(action, onRemoveSubscriptions);
            onRemoveSubscriptions.Add(subscription);
            return subscription;
        }

        public partial IDisposable SubscribeOnClear(Action action)
        {
            var subscription = new Subscription(action, onClearSubscriptions);
            onClearSubscriptions.Add(subscription);
            return subscription;
        }
        
        public partial IDisposable SubscribeToCount(Action<int> action)
        {
            var subscription = new Subscription<int>(action, countSubscriptions);
            countSubscriptions.Add(subscription); 
            return subscription;
        }

        public partial IDisposable SubscribeToValues(Action<int, T> action)
        {
            var subscription = new IndexValueSubscription<T>(action, valueSubscriptions);
            valueSubscriptions.Add(subscription);
            return subscription;
        }
        
        public partial IDisposable SubscribeToValues(Action<IndexValuePair<T>> action)
        {
            var subscription = new IndexValueSubscription<T>(action, valueSubscriptions);
            valueSubscriptions.Add(subscription);
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
        }
        
        public override void Dispose()
        {
            onAddSubscriptions.Dispose();
            onRemoveSubscriptions.Dispose();
            onClearSubscriptions.Dispose();
            countSubscriptions.Dispose();
            valueSubscriptions.Dispose();
        }
    }
    
    // Dictionary
    public abstract partial class Collection<TKey, TValue>
    {
        private readonly List<IDisposable> valueSubscriptions = new();
        
        public partial IDisposable SubscribeOnAdd(Action<TKey, TValue> action)
        {
            return base.SubscribeOnAdd(pair => action.Invoke(pair.Key, pair.Value));
        }

        public partial IDisposable SubscribeOnRemove(Action<TKey, TValue> action)
        {
            return base.SubscribeOnRemove(pair => action.Invoke(pair.Key, pair.Value));
        }

        public partial IDisposable SubscribeToValues(Action<TKey, TValue> action)
        {
            var subscription = new KeyValueSubscription<TKey, TValue>(action, valueSubscriptions);
            valueSubscriptions.Add(subscription);
            return subscription;
        }
        
        public partial IDisposable SubscribeToValues(Action<KeyValuePair<TKey, TValue>> action)
        {
            var subscription = new KeyValueSubscription<TKey, TValue>(action, valueSubscriptions);
            valueSubscriptions.Add(subscription);
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

        public override void Dispose()
        {
            valueSubscriptions.Dispose();
            base.Dispose();
        }
    }
}

#endif