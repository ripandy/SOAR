#if !SOAR_R3

using System;
using System.Collections.Generic;

namespace Soar
{
    internal struct Subscription : IDisposable
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
            action.Invoke();
        }

        public void Dispose()
        {
            action = null;
            if (disposables == null) return;
            disposables.Remove(this);
            disposables = null;
        }
    }
    
    internal struct Subscription<T> : IDisposable
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

        public void Invoke(T param)
        {
            action.Invoke(param);
        }

        public void Dispose()
        {
            action = null;
            if (disposables == null) return;
            disposables.Remove(this);
            disposables = null;
        }
    }
}

#endif