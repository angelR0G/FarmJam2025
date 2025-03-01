using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrosiveState : MonoBehaviour, IState
{
    protected const float DETECTION_DISTANCE = 1f;
    protected CorrosiveEnemyComponent enemy;

    public void Awake()
    {
        enemy = GetComponentInParent<CorrosiveEnemyComponent>();
    }

    public virtual void EnterState() { }

    public virtual void ExitState() { }

    public virtual void UpdateState() { }

    public virtual void FixedUpdateState() { }

    protected void CheckPlayerInsideDetectionRange()
    {
        PlayerComponent player = enemy.GetPlayerInRange(DETECTION_DISTANCE);

        if (player != null)
        {
            enemy.enemyTarget = player;
            enemy.ChangeState(enemy.followState);
        }
    }
}
