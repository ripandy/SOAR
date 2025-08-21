using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Soar.Transactions.Sample
{
    public class RequestSample : MonoBehaviour
    {
        [SerializeField] private FloatTransaction dummyProcessTransaction;
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text processValue;
        private TMP_Text buttonLabel;

        private void Awake()
        {
            buttonLabel = button.GetComponentInChildren<TMP_Text>();
        }

        private void Start()
        {
            button.onClick.AddListener(BeginRequest);
        }

        private async void BeginRequest()
        {
            try
            {
                button.interactable = false;
                buttonLabel.text = "Waiting for Response..";

                var responseValue = await dummyProcessTransaction.RequestAsync(Random.value * 3f);

                processValue.text = $"Time elapsed: {responseValue}"; 
            }
            catch (OperationCanceledException)
            {
                processValue.text = "Request was canceled.";
            }
            catch (Exception e)
            {
                Debug.LogError($"An error occurred while processing the request: {e}");
                processValue.text = $"Error occurred during request. {e.Message}";
            }
            finally
            {
                button.interactable = true;
                buttonLabel.text = "Request";
            }
        }
    }
}