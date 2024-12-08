#if SOAR_R3

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using R3;
using UnityEngine;

namespace Soar.Transactions
{
    public partial class Transaction
    {
        private readonly Subject<object> requestSubject = new();
        private readonly Subject<object> responseSubject = new();

        public Observable<Unit> AsRequestObservable()
        {
            return requestSubject.AsUnitObservable();
        }

        public Observable<Unit> AsResponseObservable()
        {
            return responseSubject.AsUnitObservable();
        }
        
        public IAsyncEnumerable<Unit> ToRequestAsyncEnumerable(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return AsRequestObservable().ToAsyncEnumerable(linkedTokenSource.Token);
        }
        
        public IAsyncEnumerable<Unit> ToResponseAsyncEnumerable(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return AsResponseObservable().ToAsyncEnumerable(linkedTokenSource.Token);
        }

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
            UnregisterResponse();
            registeredResponse = new ResponseRegistrar(responseAction);
            BindRegisteredResponse(AwaitOperation.Parallel);
        }

        public void RegisterResponse(Func<ValueTask> responseAsync)
        {
            RegisterResponse(responseAsync, AwaitOperation.Parallel);
        }

        public void RegisterResponse(Func<ValueTask> responseAsync, AwaitOperation awaitOperation)
        {
            UnregisterResponse();
            registeredResponse = new ResponseRegistrar(responseAsync);
            BindRegisteredResponse(awaitOperation);
        }

        public void RegisterResponse(Func<CancellationToken, ValueTask> responseAsync,
            AwaitOperation awaitOperation = AwaitOperation.Parallel)
        {
            UnregisterResponse();
            registeredResponse = new ResponseRegistrar(responseAsync);
            BindRegisteredResponse(awaitOperation);
        }

        protected void BindRegisteredResponse(AwaitOperation awaitOperation)
        {
            if (awaitOperation is AwaitOperation.SequentialParallel or AwaitOperation.ThrottleFirstLast)
            {
                Debug.LogWarning($"SOAR's Transaction implementation does not support awaitOperation {awaitOperation}. AwaitOperation is set to default {AwaitOperation.Parallel}.");
                awaitOperation = AwaitOperation.Parallel;
            }

            requestSubscription = requestSubject
                .Where(_ => IsReadyForTransaction)
                .SubscribeAwait(TryRespond, awaitOperation);
            RespondAllInternal();
            
            async ValueTask TryRespond(object _, CancellationToken token)
            {
                await RespondInternalAsync(token);
            }
        }

        internal virtual partial async ValueTask RespondInternalAsync(CancellationToken cancellationToken)
        {
            if (RequestQueueHandler.TryDequeue(out SingleAssignmentSubject<object> responseSubj))
            {
                await registeredResponse.InvokeAsync(cancellationToken);
                responseSubj.OnNext(this);
                responseSubj.Dispose();
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
            requestSubject.OnNext(this);
        }

        internal virtual partial void RaiseResponse()
        {
            responseSubject.OnNext(this);
        }

        public partial IDisposable SubscribeToRequest(Action onRequest)
        {
            return requestSubject.Subscribe(_ => onRequest.Invoke());
        }

        public partial IDisposable SubscribeToResponse(Action onResponse)
        {
            return responseSubject.Subscribe(_ => onResponse.Invoke());
        }

        public override partial void Dispose()
        {
            requestSubject.Dispose();
            responseSubject.Dispose();
            ClearRequests();
            UnregisterResponse();
        }
    }

    public abstract partial class Transaction<TRequest, TResponse>
    {
        private readonly Subject<TRequest> requestSubject = new();
        private readonly Subject<TResponse> responseSubject = new();

        public Observable<Unit> AsRequestUnitObservable()
        {
            return base.AsRequestObservable();
        }

        public Observable<Unit> AsResponseUnitObservable()
        {
            return base.AsResponseObservable();
        }

        public new Observable<TRequest> AsRequestObservable()
        {
            return requestSubject;
        }

        public new Observable<TResponse> AsResponseObservable()
        {
            return responseSubject;
        }

        public new IAsyncEnumerable<TRequest> ToRequestAsyncEnumerable(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return requestSubject.ToAsyncEnumerable(linkedTokenSource.Token);
        }

        public new IAsyncEnumerable<TRequest> ToResponseAsyncEnumerable(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return requestSubject.ToAsyncEnumerable(linkedTokenSource.Token);
        }

        public partial async ValueTask<TResponse> RequestAsync(TRequest request)
        {
            return await RequestAsync(request, Application.exitCancellationToken);
        }

        public async ValueTask<TResponse> RequestAsync(TRequest request, CancellationToken cancellationToken)
        {
            var requestTask = ValueRequestQueueHandler.EnqueueAsync(request, cancellationToken);
            RaiseRequest(request);
            return await requestTask;
        }

        public void RegisterResponse(Func<TRequest, TResponse> responseAction)
        {
            UnregisterResponse();
            registeredResponse = new ResponseRegistrar<TRequest, TResponse>(responseAction);
            BindRegisteredResponse(AwaitOperation.Parallel);
        }

        public void RegisterResponse(Func<TRequest, ValueTask<TResponse>> responseAsync)
        {
            RegisterResponse(responseAsync, AwaitOperation.Parallel);
        }

        public void RegisterResponse(Func<TRequest, ValueTask<TResponse>> responseAsync, AwaitOperation awaitOperation)
        {
            UnregisterResponse();
            registeredResponse = new ResponseRegistrar<TRequest, TResponse>(responseAsync);
            BindRegisteredResponse(awaitOperation);
        }

        public void RegisterResponse(Func<TRequest, CancellationToken, ValueTask<TResponse>> responseAsync,
            AwaitOperation awaitOperation = AwaitOperation.Parallel)
        {
            UnregisterResponse();
            registeredResponse = new ResponseRegistrar<TRequest, TResponse>(responseAsync);
            BindRegisteredResponse(awaitOperation);
        }

        internal override partial async ValueTask RespondInternalAsync(CancellationToken cancellationToken)
        {
            if (ValueRequestQueueHandler.TryDequeue(out (TRequest, SingleAssignmentSubject<TResponse>) requestAsyncTuple))
            {
                var (request, responseSubj) = requestAsyncTuple;
                if (registeredResponse is ResponseRegistrar<TRequest, TResponse> valueRegisteredResponse)
                {
                    var response = await valueRegisteredResponse.InvokeAsync(request, cancellationToken);
                    responseSubj.OnNext(response);
                    responseSubj.Dispose();
                    RaiseResponse(response);
                }
                else
                {
                    await registeredResponse.InvokeAsync(cancellationToken);
                    responseSubj.OnNext(default);
                    responseSubj.Dispose();
                    RaiseResponse();
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
                    RaiseResponse();
                }
            }
            else if (ValueRequestQueueHandler.TryDequeue(out SingleAssignmentSubject<object> responseSubj))
            {
                await registeredResponse.InvokeAsync(cancellationToken);
                responseSubj.OnNext(this);
                responseSubj.Dispose();
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
            requestSubject.OnNext(raisedRequestValue);
            base.RaiseRequest();
        }

        private partial void RaiseResponse(TResponse raisedResponseValue)
        {
            responseValue = raisedResponseValue;
            responseSubject.OnNext(raisedResponseValue);
            base.RaiseResponse();
        }
        
        public async ValueTask<TRequest> WaitRequestAsync(CancellationToken cancellationToken)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await requestSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }

        public async ValueTask<TResponse> WaitResponseAsync(CancellationToken cancellationToken)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await responseSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }

        public partial IDisposable SubscribeToRequest(Action<TRequest> action)
        {
            return requestSubject.Subscribe(action.Invoke);
        }

        public partial IDisposable SubscribeToResponse(Action<TResponse> action)
        {
            return responseSubject.Subscribe(action.Invoke);
        }

        public override void Dispose()
        {
            requestSubject.Dispose();
            responseSubject.Dispose();
            base.Dispose();
        }
    }
}

#endif
