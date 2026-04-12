using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private int speed = 5;
    [SerializeField] private int rotationSpeed = 5;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Walk(Vector3 dir)
    {
        rb.linearVelocity = dir * speed;
    }
    public void Rotate(Vector3 dir)
    {
        transform.forward = Vector3.Lerp(transform.forward, dir, rotationSpeed * Time.deltaTime);
    }
}
