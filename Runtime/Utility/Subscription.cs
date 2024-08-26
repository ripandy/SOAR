#if !SOAR_R3

using System;
using System.Collections.Generic;

namespace Soar
{
    internal class Subscription : IDisposable
    {
        private readonly Action action;
        private readonly IList<IDisposable> disposables;
        private bool disposed;
        
        public Subscription(
            Action action,
            IList<IDisposable> disposables)
        {
            this.action = action;
            this.disposables = disposables;
        }

        public void Invoke()
        {
            if (disposed) return;
            action.Invoke();
        }

        public void Dispose()
        {
            if (disposed) return;
            disposables.Remove(this);
            disposed = true;
        }
    }
    
    internal class Subscription<T> : IDisposable
    {
        private readonly Action<T> action;
        private readonly IList<IDisposable> disposables;
        private bool disposed;
        
        public Subscription(
            Action<T> action,
            IList<IDisposable> disposables)
        {
            this.action = action;
            this.disposables = disposables;
        }
        
        public void Invoke(T value)
        {
            if (disposed) return;
            action.Invoke(value);
        }

        public void Dispose()
        {
            if (disposed) return;
            disposables.Remove(this);
            disposed = true;
        }
    }
    
    internal class OldNewSubscription<T> : IDisposable
    {
        private readonly Action<T, T> action;
        private readonly IList<IDisposable> disposables;
        private bool disposed;
        
        public OldNewSubscription(
            Action<T, T> action,
            IList<IDisposable> disposables)
        {
            this.action = action;
            this.disposables = disposables;
        }
        
        public void Invoke(T oldValue, T newValue)
        {
            if (disposed) return;
            action.Invoke(oldValue, newValue);
        }
        
        public void Dispose()
        {
            if (disposed) return;
            disposables.Remove(this);
            disposed = true;
        }
    }
    
    internal class IndexValueSubscription<T> : IDisposable
    {
        private readonly Action<int, T> action;
        private readonly Action<IndexValuePair<T>> pairAction;
        private readonly IList<IDisposable> disposables;
        private bool disposed;
        
        public IndexValueSubscription(
            Action<int, T> action,
            IList<IDisposable> disposables)
        {
            this.action = action;
            this.disposables = disposables;
        }
        
        public IndexValueSubscription(
            Action<IndexValuePair<T>> action,
            IList<IDisposable> disposables)
        {
            pairAction = action;
            this.disposables = disposables;
        }
        
        public void Invoke(int index, T value)
        {
            Invoke(new IndexValuePair<T>(index, value));
        }
        
        public void Invoke(IndexValuePair<T> valuePair)
        {
            if (disposed) return;
            action?.Invoke(valuePair.Index, valuePair.Value);
            pairAction?.Invoke(valuePair);
        }
        
        public void Dispose()
        {
            if (disposed) return;
            disposables.Remove(this);
            disposed = true;
        }
    }
    
    internal class KeyValueSubscription<TKey, TValue> : IDisposable
    {
        private readonly Action<TKey, TValue> action;
        private readonly Action<KeyValuePair<TKey, TValue>> pairAction;
        private readonly IList<IDisposable> disposables;
        private bool disposed;
        
        public KeyValueSubscription(
            Action<TKey, TValue> action,
            IList<IDisposable> disposables)
        {
            this.action = action;
            this.disposables = disposables;
        }
        
        public KeyValueSubscription(
            Action<KeyValuePair<TKey, TValue>> action,
            IList<IDisposable> disposables)
        {
            pairAction = action;
            this.disposables = disposables;
        }
        
        public void Invoke(TKey key, TValue value)
        {
            Invoke(new KeyValuePair<TKey, TValue>(key, value));
        }

        public void Invoke(KeyValuePair<TKey, TValue> pair)
        {
            if (disposed) return;
            action?.Invoke(pair.Key, pair.Value);
            pairAction?.Invoke(pair);
        }
        
        public void Dispose()
        {
            if (disposed) return;
            disposables.Remove(this);
            disposed = true;
        }
    }
}

#endif