using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private LineOfSight los;
    [SerializeField] private FSM fsm;
    [SerializeField] private ObjectPickup objectPickup;

    [Header("Movement")]
    [SerializeField] private float speed = 3f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float killDistance = 1.2f;

    [Header("Patrol")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float waypointReachDistance = 0.5f;
    private int currentWaypointIndex = 0;

    [Header("Flee")]
    [SerializeField] private float fleeDetectionRange = 5f;

    private Animator animator;
    private PlayerHealth playerHealth;

    void Awake()
    {
        if (los == null) los = GetComponent<LineOfSight>();
        if (fsm == null) fsm = GetComponent<FSM>();
        animator = GetComponent<Animator>();
        playerHealth = player.GetComponent<PlayerHealth>();
    }

    void Update()
    {
        bool canSeePlayer =
            los.isInRange(transform, player)
            && los.isInAngle(transform, player)
            && los.hasLineOfSight(transform, player);

        bool playerHasScaryObject = objectPickup != null
            && objectPickup.IsHoldingScaryObject
            && Vector3.Distance(transform.position, player.position) <= fleeDetectionRange;

        fsm.UpdateState(canSeePlayer, playerHasScaryObject);
        ExecuteState();

        if (fsm.currentState != FSM.EnemyState.Flee &&
            Vector3.Distance(transform.position, player.position) <= killDistance)
            playerHealth.Die();
    }

    void ExecuteState()
    {
        switch (fsm.currentState)
        {
            case FSM.EnemyState.Patrol:  Patrol();       break;
            case FSM.EnemyState.Pursuit: PursuePlayer(); break;
            case FSM.EnemyState.Idle:    Idle();         break;
            case FSM.EnemyState.Flee:    Flee();         break;
        }
    }

    void Idle()
    {
        transform.Rotate(Vector3.up * 10f * Time.deltaTime);
        animator.SetFloat("Speed", 0f);
    }

    void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0) return;

        Transform target = waypoints[currentWaypointIndex];
        MoveToward(target.position);
        animator.SetFloat("Speed", 1f);

        if (Vector3.Distance(transform.position, target.position) <= waypointReachDistance)
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    void PursuePlayer()
    {
        MoveToward(player.position);
        animator.SetFloat("Speed", 1f);
    }

    void Flee()
    {
        Vector3 fleeDirection = (transform.position - player.position).normalized;
        Vector3 fleeTarget = transform.position + fleeDirection * 2f;
        MoveToward(fleeTarget);
        animator.SetFloat("Speed", 1f);
    }

    void MoveToward(Vector3 targetPosition)
    {
        Vector3 dir = targetPosition - transform.position;
        dir.y = 0;
        Vector3 moveDir = dir.normalized;

        transform.position += moveDir * speed * Time.deltaTime;
        transform.forward = Vector3.Lerp(transform.forward, moveDir, rotationSpeed * Time.deltaTime);
    }
}
