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
        if (enemy.attackTarget == null)
        {
            enemy.ChangeState(enemy.avoidState);
        }
        else
        {
            enemy.animator.SetTrigger("Fly");

            enemy.SetWingSoundEnabled(true);
            enemy.audioSource.clip = idleSound;
            Invoke("MakeRandomNoise", Random.Range(5f, 8f));
        }
    }

    public override void ExitState()
    {
        CancelInvoke("MakeRandomNoise");
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
            Vector3 targetVector = enemy.attackTarget.transform.position - transform.position;

            if (targetVector.magnitude < DISTANCE_TO_ATTACK)
            {
                enemy.ChangeState(enemy.attackState);
            }
            else if (targetVector.magnitude > DISTANCE_TO_TELEPORT)
            {
                enemy.ChangeState(enemy.teleportState);
            }
            else
            {
                MoveTo(targetVector.normalized, enemy.lightDetector.IsInsideLight() ? FLYING_SPEED_IN_LIGHT : FLYING_SPEED);
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
}
