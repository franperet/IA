using UnityEngine;

public class FSM : MonoBehaviour
{
    public enum EnemyState { Patrol, Pursuit, Idle, Flee }

    public EnemyState currentState = EnemyState.Patrol;

    [SerializeField] private float idleDuration = 3f;
    [SerializeField] private float fleeDuration = 5f;
    private float idleTimer = 0f;
    private float fleeTimer = 0f;

    public void UpdateState(bool canSeePlayer, bool playerHasScaryObject)
    {
        if (playerHasScaryObject && currentState != EnemyState.Flee)
        {
            currentState = EnemyState.Flee;
            fleeTimer = 0f;
            Debug.Log("Switch to Flee");
            return;
        }

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

            case EnemyState.Flee:
                fleeTimer += Time.deltaTime;
                if (fleeTimer >= fleeDuration)
                {
                    currentState = EnemyState.Patrol;
                    Debug.Log("Switch to Patrol after Flee");
                }
                break;
        }
    }
}

