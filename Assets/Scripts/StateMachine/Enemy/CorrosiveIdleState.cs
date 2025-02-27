using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrosiveIdleState : CorrosiveState
{
    private const float CHECK_PLAYER_INTERVAL = 1f;

    private float checkPlayerTimeout;

    public AudioClip idleSound;

    public override void EnterState()
    {
        enemy.animator.SetTrigger("StopMoving");

        MakeRandomNoise();
        enemy.audioSource.Stop();
    }

    public override void ExitState()
    {
        CancelInvoke("MakeRandomNoise");
        enemy.audioSource.Stop();
    }

    public override void FixedUpdateState()
    {
        checkPlayerTimeout -= Time.fixedDeltaTime;

        if (checkPlayerTimeout <= 0)
        {
            checkPlayerTimeout = CHECK_PLAYER_INTERVAL;
            CheckPlayerInsideDetectionRange();
        }

    }

    private void MakeRandomNoise()
    {
        enemy.audioSource.PlayOneShot(idleSound);

        Invoke("MakeRandomNoise", Random.Range(3f, 6f));
    }
}
