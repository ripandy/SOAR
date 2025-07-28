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
        private Subject<object> requestSubject;
        private Subject<object> RequestSubject
        {
            get
            {
                if (requestSubject == null || requestSubject.IsDisposed)
                {
                    requestSubject = new Subject<object>();
                }
                return requestSubject;
            }
        }
        private Subject<object> responseSubject;
        private Subject<object> ResponseSubject
        {
            get
            {
                if (responseSubject == null || responseSubject.IsDisposed)
                {
                    responseSubject = new Subject<object>();
                }
                return responseSubject;
            }
        }

        public Observable<Unit> AsRequestObservable()
        {
            return RequestSubject.AsUnitObservable();
        }

        public Observable<Unit> AsResponseObservable()
        {
            return ResponseSubject.AsUnitObservable();
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

            requestSubscription = RequestSubject
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
            RequestSubject.OnNext(this);
        }

        internal virtual partial void RaiseResponse()
        {
            ResponseSubject.OnNext(this);
        }

        public partial IDisposable SubscribeToRequest(Action onRequest)
        {
            return RequestSubject.Subscribe(_ => onRequest.Invoke());
        }

        public partial IDisposable SubscribeToResponse(Action onResponse)
        {
            return ResponseSubject.Subscribe(_ => onResponse.Invoke());
        }

        public override partial void Dispose()
        {
            requestSubject?.Dispose();
            responseSubject?.Dispose();
            ClearRequests();
            UnregisterResponse();
        }
    }

    public abstract partial class Transaction<TRequest, TResponse>
    {
        private Subject<TRequest> requestSubject;
        private Subject<TRequest> ValueRequestSubject
        {
            get
            {
                if (requestSubject == null || requestSubject.IsDisposed)
                {
                    requestSubject = new Subject<TRequest>();
                }
                return requestSubject;
            }
        }
        private Subject<TResponse> responseSubject;
        private Subject<TResponse> ValueResponseSubject
        {
            get
            {
                if (responseSubject == null || responseSubject.IsDisposed)
                {
                    responseSubject = new Subject<TResponse>();
                }
                return responseSubject;
            }
        }

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
            return ValueRequestSubject;
        }

        public new Observable<TResponse> AsResponseObservable()
        {
            return ValueResponseSubject;
        }

        public new IAsyncEnumerable<TRequest> ToRequestAsyncEnumerable(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return ValueRequestSubject.ToAsyncEnumerable(linkedTokenSource.Token);
        }

        public new IAsyncEnumerable<TRequest> ToResponseAsyncEnumerable(CancellationToken cancellationToken = default)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return ValueRequestSubject.ToAsyncEnumerable(linkedTokenSource.Token);
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
            ValueRequestSubject.OnNext(raisedRequestValue);
            base.RaiseRequest();
        }

        private partial void RaiseResponse(TResponse raisedResponseValue)
        {
            responseValue = raisedResponseValue;
            ValueResponseSubject.OnNext(raisedResponseValue);
            base.RaiseResponse();
        }
        
        public async ValueTask<TRequest> WaitForRequestAsync(CancellationToken cancellationToken)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await ValueRequestSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }

        public async ValueTask<TResponse> WaitForResponseAsync(CancellationToken cancellationToken)
        {
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, Application.exitCancellationToken);
            return await ValueResponseSubject.FirstOrDefaultAsync(cancellationToken: linkedTokenSource.Token);
        }

        public partial IDisposable SubscribeToRequest(Action<TRequest> onRequest)
        {
            return ValueRequestSubject.Subscribe(onRequest.Invoke);
        }

        public partial IDisposable SubscribeToResponse(Action<TResponse> onResponse)
        {
            return ValueResponseSubject.Subscribe(onResponse.Invoke);
        }

        public override void Dispose()
        {
            requestSubject?.Dispose();
            responseSubject?.Dispose();
            base.Dispose();
        }
    }
}

#endif

