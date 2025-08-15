using System;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Soar.Transactions.Sample
{
    public class ResponseSample : MonoBehaviour
    {
        [SerializeField] private FloatTransaction dummyProcessTransaction;
        
        private void Start()
        {
            dummyProcessTransaction.RegisterResponse(ProcessRequest);
        }

        private async ValueTask<float> ProcessRequest(float requestValue)
        {
            var delay = requestValue + Random.value * 3;
            await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken: destroyCancellationToken);
            return delay;
        }
    }
}