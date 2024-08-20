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
            actionOldNew = null;
            disposables?.Remove(this);
            disposables = null;
        }
    }
}

#endif