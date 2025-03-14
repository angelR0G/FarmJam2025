using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightmareFlyingState : NightmareState
{
    private const float FLYING_SPEED = 0.65f;
    private const float FLYING_SPEED_IN_LIGHT = 0.2f;
    private const float TIME_IN_LIGHT_TO_AVOID = 8f;
    private const float DISTANCE_TO_ATTACK = 0.3f;
    private const float DISTANCE_TO_TELEPORT = 3f;

    public AudioClip idleSound;

    public override void EnterState()
    {
        if (enemy.enemyTarget == null || enemy.enemyTarget.IsSafe())
        {
            enemy.ChangeState(enemy.avoidState);
        }
        else
        {
            enemy.animator.SetTrigger("Fly");

            enemy.SetWingSoundEnabled(true);
            enemy.audioSource.clip = idleSound;
            Invoke("MakeRandomNoise", Random.Range(5f, 8f));

            enemy.enemyTarget.onEnterSafeArea.AddListener(OnTargetEnterSafeArea);
        }
    }

    public override void ExitState()
    {
        CancelInvoke("MakeRandomNoise");
        enemy.enemyTarget.onEnterSafeArea.RemoveListener(OnTargetEnterSafeArea);
    }

    public override void UpdateState()
    {
        // Check if the player is inside a light
        if (IsTargetInLight()) 
            enemy.targetTimeInLight += Time.deltaTime;
        else
            enemy.targetTimeInLight = Mathf.Max(enemy.targetTimeInLight - Time.deltaTime, 0);

        // Decide what to do depending on the time inside a light and the distance to target
        if (enemy.targetTimeInLight >= TIME_IN_LIGHT_TO_AVOID)
        {
            enemy.ChangeState(enemy.avoidState);
        }
        else
        {
            float targetDistance = enemy.GetDistanceToTarget();

            if (targetDistance < DISTANCE_TO_ATTACK)
            {
                enemy.ChangeState(enemy.attackState);
            }
            else if (targetDistance > DISTANCE_TO_TELEPORT)
            {
                enemy.ChangeState(enemy.teleportState);
            }
            else
            {
                enemy.MoveTo(enemy.enemyTarget.transform.position, enemy.lightDetector.IsInsideLight() ? FLYING_SPEED_IN_LIGHT : FLYING_SPEED, true);
            }
        }
    }

    private void MakeRandomNoise()
    {
        if (!enemy.gameObject.activeSelf)
            return;

        enemy.audioSource.Play();

        Invoke("MakeRandomNoise", Random.Range(5f, 8f));
    }

    private void OnTargetEnterSafeArea()
    {
        enemy.ChangeState(enemy.avoidState);
    }
}
