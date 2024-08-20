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
        private Action<T, T> actionOldNew;
        private IList<IDisposable> disposables;
        
        public Subscription(
            Action<T> action,
            IList<IDisposable> disposables)
        {
            this.action = action;
            this.disposables = disposables;
            actionOldNew = null;
        }

        public Subscription(
            Action<T, T> action,
            IList<IDisposable> disposables)
        {
            actionOldNew = action;
            this.disposables = disposables;
            this.action = null;
        }
        
        public void Invoke(T value)
        {
            action?.Invoke(value);
        }

        public void Invoke(T oldValue, T newValue)
        {
            actionOldNew?.Invoke(oldValue, newValue);
        }

        public void Dispose()
        {
            action = null;
            actionOldNew = null;
            disposables?.Remove(this);
            disposables = null;
        }
    }
}

#endif