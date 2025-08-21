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
        private readonly CompositeDisposable subscriptions = new();

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
                
                var idx = i;
                texts[idx].text = intCollection[idx].ToString();
                intCollection.SubscribeToValues(idx, value => texts[idx].text = value.ToString()).AddTo(subscriptions);
            }

            intCollection.SubscribeOnAdd(OnCollectionAdded).AddTo(subscriptions);
            intCollection.SubscribeOnRemove(OnCollectionRemoved).AddTo(subscriptions);
        }

        private void OnCollectionAdded(int addedValue)
        {
            var idx = intCollection.Count - 1;
            texts[idx].transform.parent.gameObject.SetActive(true);
            texts[idx].text = addedValue.ToString();
            intCollection.SubscribeToValues(idx, value => texts[idx].text = value.ToString()).AddTo(subscriptions);
        }
        
        private void OnCollectionRemoved(int removedValue)
        {
            var idx = intCollection.Count;
            texts[idx].transform.parent.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            subscriptions?.Dispose();
        }
    }
}