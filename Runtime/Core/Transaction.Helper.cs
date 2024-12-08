using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#if !SOAR_R3
using System.Linq;
#endif

namespace Soar.Transactions
{
    internal class ResponseRegistrar
    {
        private readonly Action responseAction;
        private readonly Func<ValueTask> responseAsync;
        private readonly Func<CancellationToken, ValueTask> responseAsyncToken;

        internal ResponseRegistrar()
        {
            responseAction = null;
            responseAsync = null;
            responseAsyncToken = null;
        }

        public ResponseRegistrar(Action responseAction)
        {
            this.responseAction = responseAction;
            responseAsync = null;
            responseAsyncToken = null;
        }

        public ResponseRegistrar(Func<ValueTask> responseAsync)
        {
            this.responseAsync = responseAsync;
            responseAction = null;
            responseAsyncToken = null;
        }
        
        public ResponseRegistrar(Func<CancellationToken, ValueTask> responseAsyncToken)
        {
            this.responseAsyncToken = responseAsyncToken;
            responseAction = null;
            responseAsync = null;
        }

        public virtual async ValueTask InvokeAsync(CancellationToken cancellationToken = default)
        {
            if (responseAsyncToken != null)
            {
                await responseAsyncToken.Invoke(cancellationToken);
            }
            else if (responseAsync != null)
            {
                await responseAsync.Invoke();
            }
            else
            {
                responseAction.Invoke();
            }
        }
    }

    internal sealed class ResponseRegistrar<TRequest, TResponse> : ResponseRegistrar
    {
        private readonly Func<TRequest, TResponse> responseFunc;
        private readonly Func<TRequest, ValueTask<TResponse>> responseAsync;
        private readonly Func<TRequest, CancellationToken, ValueTask<TResponse>> responseAsyncToken;
        
        public ResponseRegistrar(Func<TRequest, TResponse> responseFunc)
        {
            this.responseFunc = responseFunc;
            responseAsync = null;
            responseAsyncToken = null;
        }
        
        public ResponseRegistrar(Func<TRequest, ValueTask<TResponse>> responseAsync)
        {
            this.responseAsync = responseAsync;
            responseFunc = null;
            responseAsyncToken = null;
        }
        
        public ResponseRegistrar(Func<TRequest, CancellationToken, ValueTask<TResponse>> responseAsyncToken)
        {
            this.responseAsyncToken = responseAsyncToken;
            responseFunc = null;
            responseAsync = null;
        }

        public override async ValueTask InvokeAsync(CancellationToken cancellationToken = default)
        {
            if (responseAsyncToken != null)
            {
                await responseAsyncToken.Invoke(default, cancellationToken);
            }
            else if (responseAsync != null)
            {
                await responseAsync.Invoke(default);
            }
            else if (responseFunc != null)
            {
                responseFunc.Invoke(default);
            }
            else
            {
                await base.InvokeAsync(cancellationToken);
            }
        }

        public async ValueTask<TResponse> InvokeAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            if (responseAsyncToken != null)
            {
                return await responseAsyncToken.Invoke(request, cancellationToken);
            }
            
            if (responseAsync != null)
            {
                return await responseAsync.Invoke(request);
            }
            
            if (responseFunc != null)
            {
                return responseFunc.Invoke(request);
            }

            await base.InvokeAsync(cancellationToken);
            return default;
        }
    }
    
    internal partial class RequestQueueHandler : IDisposable
    {
        private readonly Queue<Action> requestQueue = new();
        private readonly object requestQueueLock = new();
        
        public bool HasAnyRequest => EvaluateHasAnyRequest();

        public void Enqueue(Action onResponse)
        {
            lock (requestQueueLock)
            {
                requestQueue.Enqueue(onResponse);
            }
        }
        
        public bool TryDequeue(out Action onResponse)
        {
            lock (requestQueueLock)
            {
                return requestQueue.TryDequeue(out onResponse);
            }
        }
        
        // List of Partial methods. Implemented in each respective integrated Library.
        protected virtual partial bool EvaluateHasAnyRequest();
        public virtual partial void Dispose();
    }

    internal partial class RequestQueueHandler<TRequest, TResponse> : RequestQueueHandler
    {
        private readonly Queue<(TRequest request, Action<TResponse> onResponse)> valueRequestQueue = new();
        private readonly object requestQueueLock = new();
        
        public void Enqueue(TRequest request, Action<TResponse> onResponse)
        {
            lock (requestQueueLock)
            {
                valueRequestQueue.Enqueue((request, onResponse));
            }
        }
        
        public bool TryDequeue(out (TRequest request, Action<TResponse> onResponse) requestTuple)
        {
            lock (requestQueueLock)
            {
                return valueRequestQueue.TryDequeue(out requestTuple);
            }
        }
        
        // List of Partial methods. Implemented in each respective integrated Library.
        protected override partial bool EvaluateHasAnyRequest();
        public override partial void Dispose();
    }
    
#if !SOAR_R3
    
    internal partial class RequestQueueHandler
    {
        private readonly Queue<TaskCompletionSource<object>> requestAsyncQueue = new();
        
        protected virtual partial bool EvaluateHasAnyRequest()
        {
            lock (requestQueueLock)
            {
                return requestQueue.Any() || requestAsyncQueue.Any();
            }
        }
        
        public async ValueTask EnqueueAsync(CancellationToken cancellationToken = default)
        {
            // TODO: Handle Cancellation Token. Find out how to handle cancellation token on TaskCompletionSource.
            
            var tcs = new TaskCompletionSource<object>();
            lock (requestQueueLock)
            {
                requestAsyncQueue.Enqueue(tcs);
            }
            await tcs.Task;
        }

        public bool TryDequeue(out TaskCompletionSource<object> responseSubj)
        {
            lock (requestQueueLock)
            {
                return requestAsyncQueue.TryDequeue(out responseSubj);
            }
        }

        public virtual partial void Dispose()
        {
            foreach (var tcs in requestAsyncQueue)
            {
                tcs.TrySetCanceled();
            }
            requestQueue.Clear();
            requestAsyncQueue.Clear();
        }
    }
    
    internal partial class RequestQueueHandler<TRequest, TResponse>
    {
        private readonly Queue<(TRequest request, TaskCompletionSource<TResponse> responseSubj)> valueRequestAsyncQueue = new();
        
        protected override partial bool EvaluateHasAnyRequest()
        {
            lock (requestQueueLock)
            {
                return base.EvaluateHasAnyRequest() || valueRequestQueue.Any() || valueRequestAsyncQueue.Any();
            }
        }
        
        public async ValueTask<TResponse> EnqueueAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            // TODO: Handle Cancellation Token. Find out how to handle cancellation token on TaskCompletionSource.
            
            var tcs = new TaskCompletionSource<TResponse>();
            lock (requestQueueLock)
            {
                valueRequestAsyncQueue.Enqueue((request, tcs));
            }
            return await tcs.Task;
        }
        
        public bool TryDequeue(out (TRequest request, TaskCompletionSource<TResponse> responseSubj) requestTuple)
        {
            lock (requestQueueLock)
            {
                return valueRequestAsyncQueue.TryDequeue(out requestTuple);
            }
        }
        
        public override partial void Dispose()
        {
            foreach (var (_, responseTcs) in valueRequestAsyncQueue)
            {
                responseTcs.TrySetCanceled();
            }
            valueRequestQueue.Clear();
            valueRequestAsyncQueue.Clear();
            base.Dispose();
        }
    }
    
#endif
}