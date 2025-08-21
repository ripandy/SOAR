using System;
using R3;
using Soar.Events;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Soar.Collections.Sample
{
    public class CollectionEventHandler : MonoBehaviour
    {
        [SerializeField] private GameEvent onAddValueClicked;
        [SerializeField] private GameEvent onAddGridClicked;
        [SerializeField] private GameEvent onRemoveGridClicked;
        [SerializeField] private IntList intList;

        private const int MaxGrid = 8;
        
        private IDisposable subscriptions;

        private void Start()
        {
            var d1 = onAddValueClicked.Subscribe(UpdateRandomCollectionValue);
            var d2 = onAddGridClicked.Subscribe(AddGrid);
            var d3 = onRemoveGridClicked.Subscribe(RemoveGrid);
            subscriptions = Disposable.Combine(d1, d2, d3);
        }

        private void UpdateRandomCollectionValue()
        {
            if (intList.Count == 0)
                return;
            
            var randomIdx = Random.Range(0, intList.Count);
            intList[randomIdx]++;
        }

        private void AddGrid()
        {
            if (intList.Count >= MaxGrid)
                return;
            
            intList.Add(0);
        }

        private void RemoveGrid()
        {
            if (intList.Count == 0)
                return;
            
            intList.RemoveAt(intList.Count - 1);
        }
        
        private void OnDestroy()
        {
            subscriptions?.Dispose();
        }
    }
}