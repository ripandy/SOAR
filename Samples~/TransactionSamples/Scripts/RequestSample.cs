using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

        private void BeginRequest()
        {
            button.interactable = false;
            buttonLabel.text = "Waiting for Response..";
            dummyProcessTransaction.Request(0f, OnResponse);
        }

        private void OnResponse(float responseValue)
        {
            button.interactable = true;
            buttonLabel.text = "Request";
            processValue.text = $"Time elapsed: {responseValue}";
        }
    }
}