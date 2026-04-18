using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    [SerializeField] private Transform holdPoint;
    [SerializeField] private float pickupRange = 3f;
    [SerializeField] private LayerMask pickableLayer;
    [SerializeField] private InteractionUI interactionUI;

    private Rigidbody heldObject;

    public bool IsHolding => heldObject != null;

    public void Collect()
    {
        Destroy(heldObject.gameObject);
        heldObject = null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (interactionUI != null && (interactionUI.HasInteractable || interactionUI.IsPanelOpen))
                return;

            if (heldObject != null)
                Drop();
            else
                TryPickup();
        }

        if (heldObject != null)
            heldObject.MovePosition(holdPoint.position);
    }

    void TryPickup()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange, pickableLayer))
        {
            Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                heldObject = rb;
                heldObject.useGravity = false;
                heldObject.linearDamping = 10f;
                heldObject.angularDamping = 10f;
                heldObject.interpolation = RigidbodyInterpolation.Interpolate;
            }
        }
    }

    void Drop()
    {
        heldObject.useGravity = true;
        heldObject.linearDamping = 0f;
        heldObject.angularDamping = 0.05f;
        heldObject.interpolation = RigidbodyInterpolation.None;
        heldObject = null;
    }
}
