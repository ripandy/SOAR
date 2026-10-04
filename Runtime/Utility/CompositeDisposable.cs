#if !SOAR_R3

using System;
using System.Collections.Generic;

namespace Soar
{
    internal class CompositeDisposable : IDisposable
    {
        private readonly List<IDisposable> disposables = new();
        
        public void Add(IDisposable disposable)
        {
            disposables.Add(disposable);
        }

        public void Dispose()
        {
            disposables.Dispose();
        }
    }

    internal static class DisposableExtension
    {
        internal static void Dispose(this IList<IDisposable> disposables)
        {
            // NOTE: Subscription removes itself from this list when disposed.
            //       Iterate backwards to avoid skipping elements.
            for (var i = disposables.Count - 1; i >= 0; i--)
            {
                disposables[i]?.Dispose();
            }

            disposables.Clear();
        }

        internal static void AddTo(this IDisposable disposable, IList<IDisposable> disposables)
        {
            disposables.Add(disposable);
        }
        
        internal static void AddTo(this IDisposable disposable, CompositeDisposable disposables)
        {
            disposables.Add(disposable);
        }
    }
}

#endif