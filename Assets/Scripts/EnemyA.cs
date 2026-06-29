using System.Collections.Generic;
using UnityEngine;

// EnemyA - Patrullero: patrulla con Arrive, persigue con A* + Seek
[RequireComponent(typeof(LineOfSight))]
public class EnemyA : MonoBehaviour
{
    public enum State { Patrol, Pursuit }

    [Header("Referencias")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;

    [Header("Patrulla")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waypointReachDist = 1.5f;
    private int currentWaypoint = 0;

    [Header("Movimiento")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float killDistance = 1.5f;
    [SerializeField] private float arriveSlowRadius = 2f;

    [Header("Pursuit")]
    [SerializeField] private float maxChaseDistance = 30f; // distancia maxima antes de abandonar la persecucion
    private float losePlayerTimer = 0f;

    [Header("Pathfinding (A*)")]
    [SerializeField] private float nodeReachDist = 1.5f;
    [SerializeField] private float nodeWaitTime = 0.5f;
    private float nodeWaitTimer = 0f;
    private bool waitingAtNode = false;
    private List<Node> currentPath = new List<Node>();
    private int pathIndex = 0;
    private Node[] cachedNodes;

    private LineOfSight los;
    private PlayerHealth playerHealth;
    private State currentState = State.Patrol;

    void Awake()
    {
        los = GetComponent<LineOfSight>();
        if (animator == null) animator = GetComponent<Animator>();
        playerHealth = player.GetComponent<PlayerHealth>();
        cachedNodes = FindObjectsByType<Node>(FindObjectsSortMode.None);
    }

    void Update()
    {
        bool canSee = los.isInRange(transform, player)
                   && los.isInAngle(transform, player)
                   && los.hasLineOfSight(transform, player);

        UpdateFSM(canSee);
        ExecuteState();

        if (Vector3.Distance(transform.position, player.position) <= killDistance)
            playerHealth.Die();
    }

    void UpdateFSM(bool canSee)
    {
        switch (currentState)
        {
            case State.Patrol:
                if (canSee)
                {
                    currentState = State.Pursuit;
                    losePlayerTimer = 0f;
                    RequestNewPath();
                    Debug.Log("[EnemyA] Jugador detectado - iniciando Pursuit");
                }
                break;

            case State.Pursuit:
                float distToPlayer = Vector3.Distance(transform.position, player.position);
                if (distToPlayer > maxChaseDistance)
                {
                    currentState = State.Patrol;
                    Debug.Log("[EnemyA] Jugador demasiado lejos - volviendo a Patrol");
                }
                break;
        }
    }

    void ExecuteState()
    {
        switch (currentState)
        {
            case State.Patrol:  DoPatrol();  break;
            case State.Pursuit: DoPursuit(); break;
        }
    }

    void DoPatrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;
        animator?.SetFloat("Speed", 1f);

        Transform wp = waypoints[currentWaypoint];
        Vector3 flatTarget = new Vector3(wp.position.x, transform.position.y, wp.position.z);
        Vector3 vel = SteeringBehaviors.Arrive(transform.position, flatTarget, speed, arriveSlowRadius);
        MoveWithVelocity(vel);

        if (FlatDistance(transform.position, wp.position) <= waypointReachDist)
            currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
    }

    void DoPursuit()
    {
        animator?.SetFloat("Speed", 1f);

        if (currentPath.Count > 0 && pathIndex < currentPath.Count)
        {
            Vector3 nodePos = currentPath[pathIndex].transform.position;
            float distToNode = FlatDistance(transform.position, nodePos);

            if (waitingAtNode)
            {
                nodeWaitTimer -= Time.deltaTime;
                if (nodeWaitTimer <= 0f)
                {
                    waitingAtNode = false;
                    pathIndex++;
                    Debug.Log($"[EnemyA] Espera terminada - avanzando al nodo {pathIndex + 1}");
                }
            }
            else
            {
                Debug.Log($"[EnemyA] Yendo al nodo {pathIndex + 1}/{currentPath.Count} - distancia: {distToNode:F2}");
                Vector3 vel = SteeringBehaviors.Seek(transform.position, nodePos, speed);
                MoveWithVelocity(vel);

                if (distToNode <= nodeReachDist)
                {
                    Debug.Log($"[EnemyA] Nodo {pathIndex + 1} alcanzado - esperando {nodeWaitTime}s");
                    waitingAtNode = true;
                    nodeWaitTimer = nodeWaitTime;
                }
            }
        }
        else
        {
            Debug.Log($"[EnemyA] Camino terminado - yendo directo al jugador");
            Vector3 vel = SteeringBehaviors.Seek(transform.position, player.position, speed);
            MoveWithVelocity(vel);
        }
    }

    void RequestNewPath()
    {
        Node start = FindNearestNode(transform.position);
        Node goal  = FindNearestNode(player.position);
        if (start == null || goal == null) return;

        currentPath = AStar.Run(
            start,
            n => n == goal,
            n => n.neighbors,
            (a, b) => Vector3.Distance(a.transform.position, b.transform.position),
            n => Vector3.Distance(n.transform.position, goal.transform.position)
        );
        pathIndex = 0;
        waitingAtNode = false;
        nodeWaitTimer = 0f;
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
        if (currentPath == null) return;
        Gizmos.color = Color.blue;
        for (int i = 0; i < currentPath.Count - 1; i++)
            if (currentPath[i] != null && currentPath[i + 1] != null)
                Gizmos.DrawLine(currentPath[i].transform.position, currentPath[i + 1].transform.position);
    }
}
