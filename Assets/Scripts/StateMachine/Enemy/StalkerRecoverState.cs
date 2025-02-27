using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerRecoverState : StalkerState
{
    private float recoveringTime;

    public override void EnterState()
    {
        recoveringTime = 2.5f;

        enemy.animator.SetTrigger("Recover");
    }

    public override void ExitState()
    {
        enemy.positionedAudioComp.VolumeMultiplier = 1f;
    }

    public override void UpdateState()
    {
        recoveringTime -= Time.deltaTime;

        if (recoveringTime <= 0f)
            enemy.ChangeState(enemy.chaseState);
        
        else if (enemy.audioSource.isPlaying)
        {
            enemy.positionedAudioComp.VolumeMultiplier -= Time.deltaTime;
        }
    }
}
