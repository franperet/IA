using UnityEngine;

public class FSM : MonoBehaviour
{
    public enum EnemyState { Patrol, Pursuit, Idle }

    public EnemyState currentState = EnemyState.Patrol;

    [SerializeField] private float idleDuration = 3f;
    private float idleTimer = 0f;

    public void UpdateState(bool canSeePlayer)
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                if (canSeePlayer)
                {
                    currentState = EnemyState.Pursuit;
                    Debug.Log("Switch to Pursuit");
                }
                break;

            case EnemyState.Pursuit:
                if (!canSeePlayer)
                {
                    currentState = EnemyState.Idle;
                    idleTimer = 0f;
                    Debug.Log("Switch to Idle");
                }
                break;

            case EnemyState.Idle:
                if (canSeePlayer)
                {
                    currentState = EnemyState.Pursuit;
                    Debug.Log("Switch to Pursuit");
                }
                else
                {
                    idleTimer += Time.deltaTime;
                    if (idleTimer >= idleDuration)
                    {
                        currentState = EnemyState.Patrol;
                        Debug.Log("Switch to Patrol");
                    }
                }
                break;
        }
    }
}

