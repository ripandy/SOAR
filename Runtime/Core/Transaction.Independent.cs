#if !SOAR_R3

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Soar.Transactions
{
    public partial class Transaction
    {
        protected readonly List<IDisposable> requestSubscriptions = new();
        protected readonly List<IDisposable> responseSubscriptions = new();
        
        public partial async ValueTask RequestAsync()
        {
            await RequestAsync(Application.exitCancellationToken);
        }
        
        public async ValueTask RequestAsync(CancellationToken cancellationToken)
        {
            var request = RequestQueueHandler.EnqueueAsync(cancellationToken);
            RaiseRequest();
            await request;
        }

        public void RegisterResponse(Action responseAction)
        {
            registeredResponse = new ResponseRegistrar(responseAction);
            requestSubscription = SubscribeToRequest(RespondInternalAsync);
            RespondAllInternal();
        }

        public void RegisterResponse(Func<ValueTask> responseAsync)
        {
            registeredResponse = new ResponseRegistrar(responseAsync);
            requestSubscription = SubscribeToRequest(RespondInternalAsync);
            RespondAllInternal();
        }

        internal void RespondInternalAsync()
        {
            RespondInternalAsync(Application.exitCancellationToken);
        }
        
        internal virtual partial async ValueTask RespondInternalAsync(CancellationToken cancellationToken)
        {
            if (!IsReadyForTransaction)
            {
                return;
            }
            
            if (RequestQueueHandler.TryDequeue(out TaskCompletionSource<object> responseSubj))
            {
                await registeredResponse.InvokeAsync(cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    responseSubj.TrySetCanceled(cancellationToken);
                    return;
                }
                responseSubj.TrySetResult(this);
                RaiseResponse();
            }
            else if (RequestQueueHandler.TryDequeue(out Action onResponse))
            {
                await registeredResponse.InvokeAsync(cancellationToken);
                onResponse.Invoke();
                RaiseResponse();
            }
            else
            {
                throw new InvalidOperationException("Transaction could not be handled properly.");
            }
        }

        internal virtual partial void RaiseRequest()
        {
            foreach (var disposable in requestSubscriptions)
            {
                if (disposable is not Subscription subscription) continue;
                subscription.Invoke();
            }
        }
        
        internal virtual partial void RaiseResponse()
        {
            foreach (var disposable in responseSubscriptions)
            {
                if (disposable is not Subscription subscription) continue;
                subscription.Invoke();
            }
        }
        
        public partial IDisposable SubscribeToRequest(Action onRequest)
        {
            var subscription = new Subscription(onRequest, requestSubscriptions);
            requestSubscriptions.Add(subscription);
            return subscription;
        }
        
        public partial IDisposable SubscribeToResponse(Action onResponse)
        {
            var subscription = new Subscription(onResponse, responseSubscriptions);
            responseSubscriptions.Add(subscription);
            return subscription;
        }

        public override partial void Dispose()
        {
            RequestQueueHandler.Dispose();
            foreach (var subscription in requestSubscriptions)
            {
                subscription.Dispose();
            }
            foreach (var subscription in responseSubscriptions)
            {
                subscription.Dispose();
            }
            requestSubscriptions.Clear();
            responseSubscriptions.Clear();
            UnregisterResponse();
        }
    }
    
    public abstract partial class Transaction<TRequest, TResponse>
    {
        public partial async ValueTask<TResponse> RequestAsync(TRequest request)
        {
            var requestTask = ValueRequestQueueHandler.EnqueueAsync(request);
            RaiseRequest(request);
            return await requestTask;
        }
        
        public void RegisterResponse(Func<TRequest, TResponse> responseFunc)
        {
            registeredResponse = new ResponseRegistrar<TRequest,TResponse>(responseFunc);
            requestSubscription = SubscribeToRequest(RespondInternalAsync);
            RespondAllInternal();
        }

        public void RegisterResponse(Func<TRequest, ValueTask<TResponse>> responseAsync)
        {
            registeredResponse = new ResponseRegistrar<TRequest,TResponse>(responseAsync);
            requestSubscription = SubscribeToRequest(RespondInternalAsync);
            RespondAllInternal();
        }

        internal override partial async ValueTask RespondInternalAsync(CancellationToken cancellationToken)
        {
            if (!IsReadyForTransaction)
            {
                return;
            }
            
            if (ValueRequestQueueHandler.TryDequeue(out (TRequest, TaskCompletionSource<TResponse>) requestAsyncTuple))
            {
                var (request, responseTcs) = requestAsyncTuple;
                if (registeredResponse is ResponseRegistrar<TRequest, TResponse> valueRegisteredResponse)
                {
                    var response = await valueRegisteredResponse.InvokeAsync(request, cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        responseTcs.TrySetCanceled(cancellationToken);
                        return;
                    }
                    responseTcs.TrySetResult(response);
                    RaiseResponse(response);
                }
                else
                {
                    await registeredResponse.InvokeAsync(cancellationToken);
                    if (cancellationToken.IsCancellationRequested)
                    {
                        responseTcs.TrySetCanceled(cancellationToken);
                        return;
                    }
                    responseTcs.TrySetResult(default);
                    RaiseResponse(default);
                }
            }
            else if (ValueRequestQueueHandler.TryDequeue(out (TRequest, Action<TResponse>) requestTuple))
            {
                var (request, onResponse) = requestTuple;
                if (registeredResponse is ResponseRegistrar<TRequest, TResponse> valueRegisteredResponse)
                {
                    var response = await valueRegisteredResponse.InvokeAsync(request, cancellationToken);
                    onResponse.Invoke(response);
                    RaiseResponse(response);
                }
                else
                {
                    await registeredResponse.InvokeAsync(cancellationToken);
                    onResponse.Invoke(default);
                    RaiseResponse(default);
                }
            }
            else if (ValueRequestQueueHandler.TryDequeue(out TaskCompletionSource<object> responseTcs))
            {
                await registeredResponse.InvokeAsync(cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    responseTcs.TrySetCanceled(cancellationToken);
                    return;
                }
                responseTcs.TrySetResult(this);
                RaiseResponse();
            }
            else if (ValueRequestQueueHandler.TryDequeue(out Action onResponse))
            {
                await registeredResponse.InvokeAsync(cancellationToken);
                onResponse.Invoke();
                RaiseResponse();
            }
            else
            {
                throw new InvalidOperationException("Transaction could not be handled properly.");
            }
        }
        
        private partial void RaiseRequest(TRequest raisedRequestValue)
        {
            requestValue = raisedRequestValue;
            base.RaiseRequest();
            
            foreach (var disposable in requestSubscriptions)
            {
                if (disposable is not Subscription<TRequest> typedSubscription) continue;
                typedSubscription.Invoke(requestValue);
            }
        }
        
        private partial void RaiseResponse(TResponse raisedResponseValue)
        {
            responseValue = raisedResponseValue;
            base.RaiseResponse();
            
            foreach (var disposable in responseSubscriptions)
            {
                if (disposable is not Subscription<TResponse> typedSubscription) continue;
                typedSubscription.Invoke(responseValue);
            }
        }
        
        public partial IDisposable SubscribeToRequest(Action<TRequest> action)
        {
            var subscription = new Subscription<TRequest>(action, requestSubscriptions);
            requestSubscriptions.Add(subscription);
            return subscription;
        }
        
        public partial IDisposable SubscribeToResponse(Action<TResponse> action)
        {
            var subscription = new Subscription<TResponse>(action, responseSubscriptions);
            responseSubscriptions.Add(subscription);
            return subscription;
        }
    }
}

#endif
