using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrosiveFollowState : CorrosiveState
{
    private const float FOLLOW_SPEED = 0.35f;
    private const float UNFOLLOW_DISTANCE = 1.35f;
    private const float START_EXPLOSION_DISTANCE = 0.4f;

    public AudioClip startFollowingSound;

    public override void EnterState()
    {
        if (enemy.enemyTarget == null || enemy.enemyTarget.IsSafe())
        {
            enemy.ChangeState(enemy.returningState);
            return;
        }


        enemy.animator.SetTrigger("StartMoving");

        if (enemy.audioSource.isPlaying)
            enemy.audioSource.Stop();
        else
        {
            enemy.audioSource.PlayOneShot(startFollowingSound);
        }

        enemy.enemyTarget.onEnterSafeArea.AddListener(OnTargetEnterSafeArea);
    }

    public override void ExitState()
    {
        enemy.enemyTarget.onEnterSafeArea.RemoveListener(OnTargetEnterSafeArea);
    }

    public override void FixedUpdateState()
    {
        float distanceToTarget = enemy.GetDistanceToTarget();

        if (distanceToTarget < START_EXPLOSION_DISTANCE)
        {
            enemy.ChangeState(enemy.chargingState);
        }
        else if (distanceToTarget > UNFOLLOW_DISTANCE)
        {
            enemy.ChangeState(enemy.returningState);
        }
        else
        {
            enemy.MoveTo(enemy.enemyTarget.transform.position, FOLLOW_SPEED, true);
        }
    }

    private void OnTargetEnterSafeArea()
    {
        enemy.ChangeState(enemy.returningState);
    }
}
