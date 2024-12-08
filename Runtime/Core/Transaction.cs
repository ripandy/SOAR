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
        
        private bool HasRegisteredResponse => registeredResponse != null && requestSubscription != null;
        internal bool IsReadyForTransaction => HasRegisteredResponse && RequestQueueHandler.HasAnyRequest;
        
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
            if (!HasRegisteredResponse)
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
        
        // TODO: Summaries
        public partial ValueTask RequestAsync();
        public partial IDisposable SubscribeToRequest(Action onRequest);
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
            RaiseRequest(default);
        }

        internal override void RaiseResponse()
        {
            RaiseResponse(default);
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
        
        // TODO: Summaries
        public partial ValueTask<TResponse> RequestAsync(TRequest request);
        public partial IDisposable SubscribeToRequest(Action<TRequest> action);
        public partial IDisposable SubscribeToResponse(Action<TResponse> action);
    }

    public abstract class Transaction<T> : Transaction<T, T>
    {
    }
}