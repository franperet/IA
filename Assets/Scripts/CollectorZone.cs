using UnityEngine;

public class CollectorZone : MonoBehaviour
{
    [SerializeField] private ObjectPickup objectPickup;
    [SerializeField] private CollectionUI collectionUI;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && objectPickup.IsHolding)
        {
            objectPickup.Collect();
            collectionUI.AddObject();
        }
    }
}
