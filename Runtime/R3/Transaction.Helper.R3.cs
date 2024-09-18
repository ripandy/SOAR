#if SOAR_R3

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using R3;

namespace Soar.Transactions
{
    internal partial class RequestQueueHandler
    {
        private readonly Queue<SingleAssignmentSubject<object>> requestAsyncQueue = new();
        
        protected virtual partial bool EvaluateHasAnyRequest()
        {
            lock (requestQueueLock)
            {
                return requestQueue.Any() || requestAsyncQueue.Any();
            }
        }
        
        public async ValueTask EnqueueAsync(CancellationToken cancellationToken = default)
        {
            var subj = new SingleAssignmentSubject<object>();
            lock (requestQueueLock)
            {
                requestAsyncQueue.Enqueue(subj);
            }
            await subj.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }

        public bool TryDequeue(out SingleAssignmentSubject<object> responseSubj)
        {
            lock (requestQueueLock)
            {
                return requestAsyncQueue.TryDequeue(out responseSubj);
            }
        }

        public virtual partial void Dispose()
        {
            foreach (var subject in requestAsyncQueue)
            {
                subject.Dispose();
            }
            requestQueue.Clear();
            requestAsyncQueue.Clear();
        }
    }
    
    internal partial class RequestQueueHandler<TRequest, TResponse>
    {
        private readonly Queue<(TRequest request, SingleAssignmentSubject<TResponse> responseSubj)> valueRequestAsyncQueue = new();
        
        protected override partial bool EvaluateHasAnyRequest()
        {
            lock (requestQueueLock)
            {
                return base.EvaluateHasAnyRequest() || valueRequestQueue.Any() || valueRequestAsyncQueue.Any();
            }
        }
        
        public async ValueTask<TResponse> EnqueueAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            var subj = new SingleAssignmentSubject<TResponse>();
            lock (requestQueueLock)
            {
                valueRequestAsyncQueue.Enqueue((request, subj));
            }
            return await subj.FirstOrDefaultAsync(cancellationToken: cancellationToken);
        }
        
        public bool TryDequeue(out (TRequest request, SingleAssignmentSubject<TResponse> responseSubj) requestTuple)
        {
            lock (requestQueueLock)
            {
                return valueRequestAsyncQueue.TryDequeue(out requestTuple);
            }
        }
        
        public override partial void Dispose()
        {
            foreach (var (_, responseSubj) in valueRequestAsyncQueue)
            {
                responseSubj.Dispose();
            }
            valueRequestQueue.Clear();
            valueRequestAsyncQueue.Clear();
            base.Dispose();
        }
    }
}

#endif
