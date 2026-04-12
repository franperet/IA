using UnityEngine;

public class FSM : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol,
        Pursuit,
        Idle
    }

    public EnemyState currentState = EnemyState.Patrol;

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
                    Debug.Log("Switch to Idle");
                }

                break;

            case EnemyState.Idle:

                if (canSeePlayer)
                {
                    currentState = EnemyState.Pursuit;
                }
                else
                {
                    currentState = EnemyState.Patrol;
                }

                break;
            }    
        }
    } 

