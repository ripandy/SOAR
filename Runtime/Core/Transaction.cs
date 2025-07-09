using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Soar.Transactions
{
    /// <summary>
    /// Core Transaction System which involves Request and Response transaction.
    /// </summary>
    [CreateAssetMenu(fileName = "Transaction", menuName = MenuHelper.DefaultTransactionMenu + "Transaction")]
    public partial class Transaction : SoarCore
    {
        internal ResponseRegistrar registeredResponse;
        internal IDisposable requestSubscription;

        internal virtual RequestQueueHandler RequestQueueHandler { get; } = new();
        
        public bool IsResponseRegistered => registeredResponse != null && requestSubscription != null;
        internal bool IsReadyForTransaction => IsResponseRegistered && RequestQueueHandler.HasAnyRequest;
        
        public void Request(Action onResponse)
        {
            RequestQueueHandler.Enqueue(onResponse);
            RaiseRequest();
        }
        
        internal override void Initialize()
        {
            RegisterResponseInternal();
            ClearRequests();
            base.Initialize();
        }

        public void ClearRequests()
        {
            RequestQueueHandler.Dispose();
        }
        
        protected void RespondAllInternal()
        {
            // No response registered, could not respond.
            if (!IsResponseRegistered)
            {
                Debug.LogWarning("No respond. Will wait for response to be registered for transaction.");
                return;
            }

            while (RequestQueueHandler.HasAnyRequest)
            {
                _ = RespondInternalAsync(Application.exitCancellationToken);
            }
        }
        
        protected virtual void RegisterResponseInternal() { }
        
        public void ResetResponseInternal()
        {
            RegisterResponseInternal();
        }

        public void UnregisterResponse()
        {
            requestSubscription?.Dispose();
            requestSubscription = null;
            registeredResponse = null;
        }

        // List of Partial methods. Implemented in each respective integrated Library.
        internal virtual partial ValueTask RespondInternalAsync(CancellationToken cancellationToken);
        internal virtual partial void RaiseRequest();
        internal virtual partial void RaiseResponse();
        
        /// <summary>
        /// Request for a transaction. Will call registered response. Can be awaited.
        /// </summary>
        public partial ValueTask RequestAsync();
        
        /// <summary>
        /// Subscribe to Request event. Will listen when a request is made.
        /// </summary>
        /// <param name="onRequest">Action to be executed on event call.</param>
        /// <returns>Subscription's IDisposable. Call Dispose() to Unsubscribe.</returns>
        public partial IDisposable SubscribeToRequest(Action onRequest);
        
        /// <summary>
        /// Subscribe to Response event. Will listen when a response is made.
        /// </summary>
        /// <param name="onResponse">Action to be executed on event call.</param>
        /// <returns>Subscription's IDisposable. Call Dispose() to Unsubscribe.</returns>
        public partial IDisposable SubscribeToResponse(Action onResponse);
        
        public override partial void Dispose();
    }
    
    public abstract partial class Transaction<TRequest, TResponse> : Transaction
    {
        [SerializeField] protected TRequest requestValue;
        [SerializeField] protected TResponse responseValue;
        
        internal override RequestQueueHandler RequestQueueHandler { get; } = new RequestQueueHandler<TRequest, TResponse>();
        private RequestQueueHandler<TRequest, TResponse> ValueRequestQueueHandler => RequestQueueHandler as RequestQueueHandler<TRequest, TResponse>;
        
        public void Request(TRequest request, Action<TResponse> onResponse)
        {
            ValueRequestQueueHandler.Enqueue(request, onResponse);
            RaiseRequest(request);
        }
        
        internal override void RaiseRequest()
        {
            RaiseRequest(requestValue);
        }

        internal override void RaiseResponse()
        {
            RaiseResponse(responseValue);
        }
        
        private void ResetInternal()
        {
            requestValue = default;
            responseValue = default;
        }
        
        internal override void OnQuit()
        {
            ResetInternal();
            base.OnQuit();
        }
        
        // List of Partial methods. Implemented in each respective integrated Library.
        internal override partial ValueTask RespondInternalAsync(CancellationToken cancellationToken);
        private partial void RaiseRequest(TRequest raisedRequestValue);
        private partial void RaiseResponse(TResponse raisedResponseValue);
        
        /// <summary>
        /// Request for a transaction. Will call registered response. Can be awaited.
        /// </summary>
        /// <param name="request">Request value to be passed.</param>
        public partial ValueTask<TResponse> RequestAsync(TRequest request);
        
        /// <summary>
        /// Subscribe to Request event. Will listen when a request is made and is called with the request value.
        /// </summary>
        /// <param name="onRequest">Action to be executed on event call.</param>
        /// <returns>Subscription's IDisposable. Call Dispose() to Unsubscribe.</returns>
        public partial IDisposable SubscribeToRequest(Action<TRequest> onRequest);
        
        /// <summary>
        /// Subscribe to Response event. Will listen when a response is made and is called with the response value.
        /// </summary>
        /// <param name="onResponse">Action to be executed on event call.</param>
        /// <returns>Subscription's IDisposable. Call Dispose() to Unsubscribe.</returns>
        public partial IDisposable SubscribeToResponse(Action<TResponse> onResponse);
    }

    public abstract class Transaction<T> : Transaction<T, T>
    {
    }
}