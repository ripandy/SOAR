#if !SOAR_R3

using System;
using System.Collections.Generic;

namespace Soar
{
    internal class Subscription : IDisposable
    {
        private Action action;
        private IList<IDisposable> disposables;
        
        public Subscription(
            Action action,
            IList<IDisposable> disposables)
        {
            this.action = action;
            this.disposables = disposables;
        }

        public void Invoke()
        {
            action?.Invoke();
        }

        public void Dispose()
        {
            action = null;
            disposables?.Remove(this);
            disposables = null;
        }
    }
    
    internal class Subscription<T> : IDisposable
    {
        private Action<T> action;
        private IList<IDisposable> disposables;
        
        public Subscription(
            Action<T> action,
            IList<IDisposable> disposables)
        {
            this.action = action;
            this.disposables = disposables;
        }
        
        public void Invoke(T value)
        {
            action?.Invoke(value);
        }

        public void Dispose()
        {
            action = null;
            disposables?.Remove(this);
            disposables = null;
        }
    }
    
    internal class OldNewSubscription<T> : IDisposable
    {
        private Action<T, T> action;
        private IList<IDisposable> disposables;
        
        public OldNewSubscription(
            Action<T, T> action,
            IList<IDisposable> disposables)
        {
            this.action = action;
            this.disposables = disposables;
        }
        
        public void Invoke(T oldValue, T newValue)
        {
            action?.Invoke(oldValue, newValue);
        }
        
        public void Dispose()
        {
            action = null;
            disposables?.Remove(this);
            disposables = null;
        }
    }
    
    internal class IndexValueSubscription<T> : IDisposable
    {
        private Action<int, T> action;
        private IList<IDisposable> disposables;
        
        public IndexValueSubscription(
            Action<int, T> action,
            IList<IDisposable> disposables)
        {
            this.action = action;
            this.disposables = disposables;
        }
        
        public void Invoke(int index, T value)
        {
            action?.Invoke(index, value);
        }
        
        public void Dispose()
        {
            action = null;
            disposables?.Remove(this);
            disposables = null;
        }
    }
    
    internal class KeyValueSubscription<TKey, TValue> : IDisposable
    {
        private Action<TKey, TValue> action;
        private IList<IDisposable> disposables;
        
        public KeyValueSubscription(
            Action<TKey, TValue> action,
            IList<IDisposable> disposables)
        {
            this.action = action;
            this.disposables = disposables;
        }
        
        public void Invoke(TKey key, TValue value)
        {
            action?.Invoke(key, value);
        }
        
        public void Dispose()
        {
            action = null;
            disposables?.Remove(this);
            disposables = null;
        }
    }
}

#endif