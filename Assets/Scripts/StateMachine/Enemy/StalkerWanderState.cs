using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerWanderState : StalkerState
{
    private const float WALKING_SPEED = 0.5f;
    private const float TARGET_REACHED_TRESHOLD = 0.1f;
    private const float STAND_STILL_TIME = 5f;
    private const float MAX_TIME_TO_REACH_TARGET = 20f;
    private const float DETECTION_DISTANCE = 1.5f;

    public Vector3 targetPosition;
    private float nextTargetCountdown;
    private bool hasReachedTarget;
    private float playerDetectionCountdown;

    public AudioClip idleSound;
    private Sequence soundSequence;

    public override void EnterState()
    {
        nextTargetCountdown = STAND_STILL_TIME;
        hasReachedTarget = false;
        UpdateTargetReached(true);

        if (enemy.audioSource.isPlaying)
        {
            soundSequence = DOTween.Sequence();

            soundSequence.Append(DOTween.To(() => enemy.positionedAudioComp.VolumeMultiplier, (v) => enemy.positionedAudioComp.VolumeMultiplier = v, 0, 1))
                .AppendCallback(MakeRandomNoise)
                .AppendCallback(enemy.audioSource.Stop)
                .OnKill(() => { soundSequence = null; enemy.positionedAudioComp.VolumeMultiplier = 1; });
        }
        else
        {
            MakeRandomNoise();
            enemy.audioSource.Stop();
        }
    }

    public override void ExitState()
    {
        CancelInvoke("MakeRandomNoise");

        if (soundSequence != null && soundSequence.IsActive())
        {
            soundSequence.Kill();
        }
    }

    public override void FixedUpdateState()
    {
        UpdateMovementTimeout();
        MoveToTarget();

        if (playerDetectionCountdown > 0)
            playerDetectionCountdown -= Time.fixedDeltaTime;
        else
        {
            playerDetectionCountdown = 0.2f;
            CheckPlayerInSight();
        }
    }

    private void UpdateTargetPosition()
    {
        targetPosition = MapComponent.Instance.GetWalkablePositionAroundPoint(transform.position, 0.5f, 2f);
        nextTargetCountdown = MAX_TIME_TO_REACH_TARGET;
        UpdateTargetReached(false);
    }

    private void UpdateMovementTimeout()
    {
        nextTargetCountdown -= Time.fixedDeltaTime;

        if (nextTargetCountdown <= 0f)
        {
            UpdateTargetPosition();
        }
    }

    private void MoveToTarget()
    {
        if (hasReachedTarget || enemy.healthComp.IsStunned())
            return;

        // Check if it is in the target position
        Vector3 targetVector = targetPosition - transform.position;
        if (targetVector.magnitude < TARGET_REACHED_TRESHOLD)
        {
            UpdateTargetReached(true);
        }
        else
        {
            enemy.MoveTo(targetPosition, Mathf.Min(targetVector.magnitude, WALKING_SPEED), false);
        }
    }

    private void UpdateTargetReached(bool isInTargetPosition)
    {
        if (hasReachedTarget == isInTargetPosition) return;

        hasReachedTarget = isInTargetPosition;

        if (hasReachedTarget)
            nextTargetCountdown = STAND_STILL_TIME;

        enemy.animator.SetTrigger(hasReachedTarget ? "StopMoving" : "StartMoving");
    }

    private void CheckPlayerInSight()
    {
        PlayerComponent player = enemy.GetPlayerInRange(DETECTION_DISTANCE);

        if (player != null && enemy.IsGameObjectInSight(player.gameObject))
        {
            enemy.enemyTarget = player;
            enemy.ChangeState(enemy.stalkState);
        }
    }

    private void MakeRandomNoise()
    {
        if (!enemy.gameObject.activeSelf)
            return;

        enemy.audioSource.PlayOneShot(idleSound);

        Invoke("MakeRandomNoise", Random.Range(4f, 7f));
    }
}
