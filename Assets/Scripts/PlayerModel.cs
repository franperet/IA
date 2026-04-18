using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    private Rigidbody rb;
    [SerializeField] private int speed = 5;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Walk(Vector3 dir)
    {
        Vector3 velocity = dir * speed;
        velocity.y = rb.linearVelocity.y; // preserva gravedad
        rb.linearVelocity = velocity;
    }

    public void RotateY(float angle)
    {
        transform.Rotate(0f, angle, 0f);
    }
}
