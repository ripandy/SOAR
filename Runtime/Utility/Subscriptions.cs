#if !SOAR_R3

using System;
using System.Collections.Generic;

namespace Soar
{
    internal sealed class Subscription : IDisposable
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
    
    internal sealed class Subscription<T> : IDisposable
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
    
    internal sealed class PairwiseSubscription<T> : IDisposable
    {
        private readonly Action<T, T> action;
        private readonly IList<IDisposable> disposables;
        private bool disposed;
        
        public PairwiseSubscription(
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
    
    internal sealed class IndexValueSubscription<T> : IDisposable
    {
        private readonly Action<T> action;
        private readonly Action<int, T> indexAction;
        private readonly Action<IndexValuePair<T>> pairAction;
        private readonly IList<IDisposable> disposables;
        private readonly int index;
        private bool disposed;
        
        public IndexValueSubscription(
            int index,
            Action<T> action,
            IList<IDisposable> disposables)
        {
            this.index = index;
            this.action = action;
            this.disposables = disposables;
        }
        
        public IndexValueSubscription(
            Action<int, T> indexAction,
            IList<IDisposable> disposables)
        {
            this.indexAction = indexAction;
            this.disposables = disposables;
        }
        
        public IndexValueSubscription(
            Action<IndexValuePair<T>> pairAction,
            IList<IDisposable> disposables)
        {
            this.pairAction = pairAction;
            this.disposables = disposables;
        }
        
        public void Invoke(int idx, T value)
        {
            if (disposed) return;

            var valuePair = new IndexValuePair<T>(idx, value);
            indexAction?.Invoke(valuePair.Index, valuePair.Value);
            pairAction?.Invoke(valuePair);
            
            if (index != idx) return;
            action?.Invoke(valuePair.Value);
        }
        
        public void Dispose()
        {
            if (disposed) return;
            disposables.Remove(this);
            disposed = true;
        }
    }
    
    internal sealed class MoveValueSubscription<T> : IDisposable
    {
        private readonly Action<T, int, int> action;
        private readonly Action<MovedValueDto<T>> moveAction;
        private readonly IList<IDisposable> disposables;
        private bool disposed;
        
        public MoveValueSubscription(
            Action<T, int, int> action,
            IList<IDisposable> disposables)
        {
            this.action = action;
            this.disposables = disposables;
        }
        
        public MoveValueSubscription(
            Action<MovedValueDto<T>> action,
            IList<IDisposable> disposables)
        {
            moveAction = action;
            this.disposables = disposables;
        }
        
        public void Invoke(T value, int oldIndex, int newIndex)
        {
            Invoke(new MovedValueDto<T>(value, oldIndex, newIndex));
        }
        
        public void Invoke(MovedValueDto<T> movedValueDto)
        {
            if (disposed) return;
            action?.Invoke(movedValueDto.Value, movedValueDto.OldIndex, movedValueDto.NewIndex);
            moveAction?.Invoke(movedValueDto);
        }
        
        public void Dispose()
        {
            if (disposed) return;
            disposables.Remove(this);
            disposed = true;
        }
    }
    
    internal sealed class KeyValueSubscription<TKey, TValue> : IDisposable
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