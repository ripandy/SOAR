using System;
using UnityEngine;

namespace Soar.Transactions.Sample
{
    public class ResponseSample : MonoBehaviour
    {
        [SerializeField] private FloatTransaction dummyProcessTransaction;

        private void Start()
        {
            dummyProcessTransaction.RegisterResponse(ProcessRequest);
        }

        private static float ProcessRequest(float requestValue)
        {
            var startTime = DateTime.Now;
            var loopCounter = 0;
            for (var i = 0; i < 999999999; i++)
            {
                loopCounter++;
            }
            var responseValue = (float)(DateTime.Now - startTime).TotalSeconds;
            Debug.Log($"looped {loopCounter} times.");
            return responseValue;
        }
    }
}