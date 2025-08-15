using System.Collections.Generic;
using R3;
using TMPro;
using UnityEngine;

namespace Soar.Collections.Sample
{
    public class CollectionViewHandler : MonoBehaviour
    {
        [SerializeField] private IntList intCollection;

        private readonly List<TMP_Text> texts = new();
        private CompositeDisposable subscriptions = new();

        private void Awake()
        {
            GetComponentsInChildren(true, texts);
        }

        private void Start()
        {
            for (var i = 0; i < texts.Count; i++)
            {
                var isVisible = i < intCollection.Count;
                texts[i].transform.parent.gameObject.SetActive(isVisible);
                
                if (!isVisible) continue;
                
                Debug.Log($"[{GetType().Name}] Subscribing to index {i} with value {intCollection[i]}");
                var idx = i;
                intCollection.SubscribeToValues(idx, value =>
                {
                    Debug.Log($"[{GetType().Name}] Value at index {idx} changed to {value}");
                    texts[idx].text = value.ToString();
                }).AddTo(subscriptions);
            }

            intCollection.SubscribeOnAdd(OnCollectionAdded).AddTo(subscriptions);
            intCollection.SubscribeOnRemove(OnCollectionRemoved).AddTo(subscriptions);
        }

        private void OnCollectionAdded(int addedValue)
        {
            var idx = intCollection.Count - 1;
            texts[idx].transform.parent.gameObject.SetActive(true);
            Debug.Log($"[{GetType().Name}] Value at index {idx} added with value {addedValue}. Subscribing to index {idx} with value {intCollection[idx]}");
            intCollection.SubscribeToValues(idx, value =>
            {
                Debug.Log($"[{GetType().Name}] Value at index {idx} changed to {value}");
                texts[idx].text = value.ToString();
            }).AddTo(subscriptions);
        }
        
        private void OnCollectionRemoved(int removedValue)
        {
            var idx = intCollection.Count;
            texts[idx].transform.parent.gameObject.SetActive(false);
            Debug.Log($"[{GetType().Name}] Value at index {idx} removed.");
        }

        private void OnDestroy()
        {
            subscriptions?.Dispose();
            subscriptions = null;
            texts.Clear();
        }
    }
}