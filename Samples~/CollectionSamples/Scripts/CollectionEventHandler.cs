using Soar.Events;
using UnityEngine;

namespace Soar.Collections.Sample
{
    public class CollectionEventHandler : MonoBehaviour
    {
        [SerializeField] private GameEvent onAddValueClicked;
        [SerializeField] private GameEvent onAddGridClicked;
        [SerializeField] private GameEvent onRemoveGridClicked;
        [SerializeField] private IntList intList;

        private const int MaxGrid = 8;

        private void Start()
        {
            onAddValueClicked.Subscribe(UpdateRandomCollectionValue);
            onAddGridClicked.Subscribe(AddGrid);
            onRemoveGridClicked.Subscribe(RemoveGrid);
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
    }
}