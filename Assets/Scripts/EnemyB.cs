using UnityEngine;

// EnemyB - Cobarde: deambula con Wander y huye con Flee si el jugador se acerca
public class EnemyB : MonoBehaviour
{
    public enum State { Wander, Flee }

    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;

    [Header("Movimiento")]
    [SerializeField] private float wanderSpeed = 2f;
    [SerializeField] private float fleeSpeed = 4f;
    [SerializeField] private float rotationSpeed = 4f;

    [Header("Deteccion")]
    [SerializeField] private float fleeRadius = 8f;
    [SerializeField] private float killDistance = 1.2f;

    [Header("Limites del mapa")]
    [SerializeField] private Vector2 minBounds = new Vector2(-50f, -50f);
    [SerializeField] private Vector2 maxBounds = new Vector2(50f, 50f);

    private State currentState = State.Wander;
    private float wanderAngle = 0f;
    private PlayerHealth playerHealth;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        playerHealth = player.GetComponent<PlayerHealth>();
        wanderAngle = Random.Range(0f, 360f);
    }

    void Update()
    {
        float dist = Vector3.Distance(transform.position, player.position);
        State newState = dist <= fleeRadius ? State.Flee : State.Wander;

        if (newState != currentState)
        {
            if (newState == State.Flee)
                Debug.Log("[EnemyB] Jugador cerca - escapando");
            else
                Debug.Log("[EnemyB] Jugador lejos - volviendo a Wander");
            currentState = newState;
        }

        Vector3 vel = Vector3.zero;

        switch (currentState)
        {
            case State.Wander:
                vel = SteeringBehaviors.Wander(ref wanderAngle, wanderSpeed);
                animator?.SetFloat("Speed", 0.5f);
                break;

            case State.Flee:
                vel = SteeringBehaviors.Flee(transform.position, player.position, fleeSpeed);
                animator?.SetFloat("Speed", 1f);
                break;
        }

        MoveWithVelocity(vel);

        if (dist <= killDistance)
            playerHealth.Die();
    }

    void MoveWithVelocity(Vector3 vel)
    {
        vel.y = 0;
        if (vel.sqrMagnitude < 0.001f) return;

        Vector3 newPos = transform.position + vel * Time.deltaTime;

        // Si sale de los limites, girar 180 grados
        if (newPos.x < minBounds.x || newPos.x > maxBounds.x ||
            newPos.z < minBounds.y || newPos.z > maxBounds.y)
        {
            wanderAngle += 180f;
            return;
        }

        transform.position = newPos;
        transform.forward = Vector3.Lerp(transform.forward, vel.normalized, rotationSpeed * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, fleeRadius);

        // Mostrar limites en la escena
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((minBounds.x + maxBounds.x) / 2, transform.position.y, (minBounds.y + maxBounds.y) / 2);
        Vector3 size = new Vector3(maxBounds.x - minBounds.x, 1f, maxBounds.y - minBounds.y);
        Gizmos.DrawWireCube(center, size);
    }
}
