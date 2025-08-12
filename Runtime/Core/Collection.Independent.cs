#if !SOAR_R3

using System;
using System.Collections.Generic;

namespace Soar.Collections
{
    public abstract partial class Collection<T>
    {
        private readonly List<IDisposable> onAddSubscriptions = new();
        private readonly List<IDisposable> onRemoveSubscriptions = new();
        private readonly List<IDisposable> onClearSubscriptions = new();
        private readonly List<IDisposable> countSubscriptions = new();
        private readonly List<IDisposable> valueSubscriptions = new();

        internal partial void RaiseOnAdd(T addedValue)
        {
            foreach (var disposable in onAddSubscriptions)
            {
                if (disposable is Subscription<T> valueSubscription)
                {
                    valueSubscription.Invoke(addedValue);
                }
            }
        }
        
        internal partial void RaiseOnRemove(T removedValue)
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
        
        internal partial void RaiseCount()
        {
            foreach (var disposable in countSubscriptions)
            {
                if (disposable is Subscription<int> countSubscription)
                {
                    countSubscription.Invoke(Count);
                }
            }
        }

        internal partial void RaiseValueAt(int index, T value)
        {
            foreach (var disposable in valueSubscriptions)
            {
                if (disposable is IndexValueSubscription<T> valueSubscription)
                {
                    valueSubscription.Invoke(index, value);
                }
            }
        }
        
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
        
        public override void Dispose()
        {
            onAddSubscriptions.Dispose();
            onRemoveSubscriptions.Dispose();
            onClearSubscriptions.Dispose();
            countSubscriptions.Dispose();
            valueSubscriptions.Dispose();
        }
    }

    // List
    public abstract partial class SoarList<T>
    {
        private readonly List<IDisposable> moveSubscriptions = new();
        private readonly List<IDisposable> insertSubscriptions = new();

        private partial void RaiseOnMove(T value, int oldIndex, int newIndex)
        {
            foreach (var subscription in moveSubscriptions)
            {
                if (subscription is MoveValueSubscription<T> moveValueSubscription)
                {
                    moveValueSubscription.Invoke(value, oldIndex, newIndex);
                }
            }
        }
        
        private partial void RaiseOnInsert(int index, T value)
        {
            foreach (var subscription in insertSubscriptions)
            {
                if (subscription is IndexValueSubscription<T> indexValuePair)
                {
                    indexValuePair.Invoke(index, value);
                }
            }
        }
        
        public partial IDisposable SubscribeOnMove(Action<T, int, int> action)
        {
            var subscription = new MoveValueSubscription<T>(action, moveSubscriptions);
            moveSubscriptions.Add(subscription);
            return subscription;
        }
        
        public partial IDisposable SubscribeOnMove(Action<MovedValueDto<T>> action)
        {
            var subscription = new MoveValueSubscription<T>(action, moveSubscriptions);
            moveSubscriptions.Add(subscription);
            return subscription;
        }
        
        public partial IDisposable SubscribeOnInsert(Action<int, T> action)
        {
            var subscription = new IndexValueSubscription<T>(action, insertSubscriptions);
            insertSubscriptions.Add(subscription);
            return subscription;
        }
        
        public partial IDisposable SubscribeOnInsert(Action<IndexValuePair<T>> action)
        {
            var subscription = new IndexValueSubscription<T>(action, insertSubscriptions);
            insertSubscriptions.Add(subscription);
            return subscription;
        }

        public override void Dispose()
        {
            base.Dispose();
            moveSubscriptions.Dispose();
            insertSubscriptions.Dispose();
        }
    }
    
    // Dictionary
    public abstract partial class SoarDictionary<TKey, TValue>
    {
        private readonly List<IDisposable> valueSubscriptions = new();
        
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

        public override void Dispose()
        {
            valueSubscriptions.Dispose();
            base.Dispose();
        }
    }
}

#endif