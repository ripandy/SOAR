using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Soar.Collections.Sample
{
    public class CollectionViewHandler : MonoBehaviour
    {
        [SerializeField] private IntList intCollection;

        private readonly List<TMP_Text> texts = new();

        private void Awake()
        {
            GetComponentsInChildren(true, texts);
        }

        private void Start()
        {
            for (var i = 0; i < texts.Count; i++)
            {
                texts[i].transform.parent.gameObject.SetActive(i < intCollection.Count);
                
                var j = i;
                if (i < intCollection.Count)
                {
                    intCollection.SubscribeToValues((index, value) =>
                    {
                        if (index == j)
                        {
                            texts[j].text = "" + value;
                        }
                    });
                }
            }

            intCollection.SubscribeOnAdd(OnCollectionAdded);
            intCollection.SubscribeOnRemove(OnCollectionRemoved);
        }

        private void OnCollectionAdded(int addedValue)
        {
            var idx = intCollection.Count - 1;
            texts[idx].transform.parent.gameObject.SetActive(true);
            intCollection.SubscribeToValues((index, value) =>
            {
                if (index == idx)
                {
                    texts[idx].text = "" + value;
                }
            });
        }
        
        private void OnCollectionRemoved(int removedValue)
        {
            var idx = intCollection.Count;
            texts[idx].transform.parent.gameObject.SetActive(false);
        }
    }
}