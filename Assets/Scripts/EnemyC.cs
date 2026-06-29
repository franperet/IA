using System.Collections.Generic;
using UnityEngine;

// EnemyC - Guardian: protege un objeto, persigue con Dijkstra si el jugador entra a su zona
public class EnemyC : MonoBehaviour
{
    public enum State { Guard, Chase, Return }

    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform guardObject; // objeto que protege
    [SerializeField] private Animator animator;

    [Header("Movimiento")]
    [SerializeField] private float speed = 4f;
    [SerializeField] private float rotationSpeed = 6f;
    [SerializeField] private float nodeReachDist = 1.5f;

    [Header("Zona de guardia")]
    [SerializeField] private float guardRadius = 3f;   // radio de wander alrededor del objeto
    [SerializeField] private float detectionRadius = 10f; // radio para detectar al jugador
    [SerializeField] private float chaseRadius = 15f;  // radio maximo de persecucion

    [Header("Ataque")]
    [SerializeField] private float killDistance = 1.5f;

    [Header("Pathfinding (Dijkstra)")]
    [SerializeField] private float pathCooldown = 0.8f;
    private float pathCooldownTimer = 0f;
    private List<Node> currentPath = new List<Node>();
    private int pathIndex = 0;
    private Node[] cachedNodes;

    private PlayerHealth playerHealth;
    private Rigidbody playerRb;
    private State currentState = State.Guard;
    private float wanderAngle = 0f;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        playerHealth = player.GetComponent<PlayerHealth>();
        playerRb = player.GetComponent<Rigidbody>();
        cachedNodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
        wanderAngle = Random.Range(0f, 360f);
    }

    void Update()
    {
        float distToPlayer = Vector3.Distance(transform.position, player.position);
        float distToObject = Vector3.Distance(transform.position, guardObject.position);

        UpdateFSM(distToPlayer, distToObject);

        switch (currentState)
        {
            case State.Guard:  DoGuard();  break;
            case State.Chase:  DoChase();  break;
            case State.Return: DoReturn(); break;
        }

        if (distToPlayer <= killDistance)
            playerHealth.Die();
    }

    void UpdateFSM(float distToPlayer, float distToObject)
    {
        switch (currentState)
        {
            case State.Guard:
                if (distToPlayer <= detectionRadius)
                {
                    currentState = State.Chase;
                    RequestPathToPlayer();
                    Debug.Log("[EnemyC] Jugador detectado - persiguiendo");
                }
                break;

            case State.Chase:
                if (distToPlayer > chaseRadius)
                {
                    currentState = State.Return;
                    RequestPathToObject();
                    Debug.Log("[EnemyC] Jugador muy lejos - volviendo al objeto");
                }
                break;

            case State.Return:
                if (distToPlayer <= detectionRadius)
                {
                    currentState = State.Chase;
                    RequestPathToPlayer();
                    Debug.Log("[EnemyC] Jugador detectado durante retorno - persiguiendo");
                }
                else if (distToObject <= nodeReachDist)
                {
                    currentState = State.Guard;
                    Debug.Log("[EnemyC] Objeto alcanzado - volviendo a guardia");
                }
                break;
        }
    }

    void DoGuard()
    {
        animator?.SetFloat("Speed", 0.5f);

        Vector3 wanderVel = SteeringBehaviors.Wander(ref wanderAngle, speed * 0.5f);
        Vector3 newPos = transform.position + wanderVel * Time.deltaTime;

        float distFromObject = Vector2.Distance(
            new Vector2(newPos.x, newPos.z),
            new Vector2(guardObject.position.x, guardObject.position.z)
        );

        if (distFromObject > guardRadius)
        {
            // Gira hacia el objeto y vuelve
            wanderAngle += 180f;
            Vector3 returnVel = SteeringBehaviors.Arrive(transform.position, guardObject.position, speed * 0.5f, guardRadius);
            MoveWithVelocity(returnVel);
        }
        else
        {
            MoveWithVelocity(wanderVel);
        }
    }

    void DoChase()
    {
        animator?.SetFloat("Speed", 1f);
        pathCooldownTimer -= Time.deltaTime;

        if ((currentPath.Count == 0 || pathIndex >= currentPath.Count) && pathCooldownTimer <= 0f)
        {
            RequestPathToPlayer();
            pathCooldownTimer = pathCooldown;
        }

        Vector3 playerVel = playerRb != null ? playerRb.linearVelocity : Vector3.zero;

        if (currentPath.Count > 0 && pathIndex < currentPath.Count)
        {
            Vector3 nodePos = currentPath[pathIndex].transform.position;
            Vector3 vel = SteeringBehaviors.Pursue(transform.position, nodePos, playerVel, speed);
            MoveWithVelocity(vel);

            Debug.Log($"[EnemyC] Persiguiendo - nodo {pathIndex + 1}/{currentPath.Count} distancia: {FlatDistance(transform.position, nodePos):F2}");

            if (FlatDistance(transform.position, nodePos) <= nodeReachDist)
                pathIndex++;
        }
        else
        {
            Vector3 vel = SteeringBehaviors.Pursue(transform.position, player.position, playerVel, speed);
            MoveWithVelocity(vel);
        }
    }

    void DoReturn()
    {
        animator?.SetFloat("Speed", 1f);

        if (currentPath.Count > 0 && pathIndex < currentPath.Count)
        {
            Vector3 nodePos = currentPath[pathIndex].transform.position;
            Vector3 vel = SteeringBehaviors.Seek(transform.position, nodePos, speed);
            MoveWithVelocity(vel);

            Debug.Log($"[EnemyC] Volviendo al objeto - nodo {pathIndex + 1}/{currentPath.Count}");

            if (FlatDistance(transform.position, nodePos) <= nodeReachDist)
                pathIndex++;
        }
        else
        {
            // Camino terminado: Arrive directo al objeto
            Vector3 vel = SteeringBehaviors.Arrive(transform.position, guardObject.position, speed, 2f);
            MoveWithVelocity(vel);
        }
    }

    void RequestPathToPlayer()
    {
        Node start = FindNearestNode(transform.position);
        Node goal  = FindNearestNode(player.position);
        if (start == null || goal == null) return;

        currentPath = Dijkstra.Run(
            start,
            n => n == goal,
            n => n.neighbors,
            (a, b) => Vector3.Distance(a.transform.position, b.transform.position)
        );
        pathIndex = 0;
        Debug.Log($"[EnemyC] Camino al jugador calculado - {currentPath.Count} nodos");
    }

    void RequestPathToObject()
    {
        Node start = FindNearestNode(transform.position);
        Node goal  = FindNearestNode(guardObject.position);
        if (start == null || goal == null) return;

        currentPath = Dijkstra.Run(
            start,
            n => n == goal,
            n => n.neighbors,
            (a, b) => Vector3.Distance(a.transform.position, b.transform.position)
        );
        pathIndex = 0;
        Debug.Log($"[EnemyC] Camino al objeto calculado - {currentPath.Count} nodos");
    }

    Node FindNearestNode(Vector3 pos)
    {
        Node nearest = null;
        float best = float.MaxValue;
        foreach (var n in cachedNodes)
        {
            float d = Vector3.Distance(pos, n.transform.position);
            if (d < best) { best = d; nearest = n; }
        }
        return nearest;
    }

    float FlatDistance(Vector3 a, Vector3 b)
    {
        return Vector2.Distance(new Vector2(a.x, a.z), new Vector2(b.x, b.z));
    }

    void MoveWithVelocity(Vector3 vel)
    {
        vel.y = 0;
        if (vel.sqrMagnitude < 0.001f) return;
        transform.position += vel * Time.deltaTime;
        transform.forward = Vector3.Lerp(transform.forward, vel.normalized, rotationSpeed * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        if (guardObject == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(guardObject.position, guardRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(guardObject.position, detectionRadius);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(guardObject.position, chaseRadius);
    }
}
