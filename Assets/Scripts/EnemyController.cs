using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform player;

    [SerializeField]
    private LineOfSight los;

    [SerializeField]
    private FSM fsm;

    [Header("Movement")]
    [SerializeField]
    private float speed = 3f;

    [SerializeField]
    private float rotationSpeed = 5f;

    void Awake()
    {
        if (los == null)
            los = GetComponent<LineOfSight>();

        if (fsm == null)
            fsm = GetComponent<FSM>();
    }

    void Idle()
    {
        // Se queda quieto pero rota un poco
        transform.Rotate(Vector3.up * 10f * Time.deltaTime);
    }

    void Update()
    {
        bool canSeePlayer =
            los.isInRange(transform, player)
            && los.isInAngle(transform, player)
            && los.hasLineOfSight(transform, player);

        fsm.UpdateState(canSeePlayer);

        ExecuteState();
    }

    void ExecuteState()
    {
        switch (fsm.currentState)
        {
            case FSM.EnemyState.Patrol:
                Patrol();
                break;

            case FSM.EnemyState.Pursuit:
                PursuePlayer();
                break;

            case FSM.EnemyState.Idle:
                Idle();
                break;
        }
    }

    // private void ExecuteState()
    // {
    //     if (fsm.CurrentState is PatrolState)
    //     {
    //         Patrol();
    //     }
    //     else if (fsm.CurrentState is PursuitState)
    //     {
    //         PursuePlayer();
    //     }
    // }

    void Patrol()
    {
        transform.Rotate(Vector3.up * 30f * Time.deltaTime);
    }

    void PursuePlayer()
    {
        Vector3 dir = player.position - transform.position;
        dir.y = 0;

        Vector3 moveDir = dir.normalized;

        transform.position += moveDir * speed * Time.deltaTime;

        transform.forward = Vector3.Lerp(
            transform.forward,
            moveDir,
            Time.deltaTime * rotationSpeed
        );
    }
}
