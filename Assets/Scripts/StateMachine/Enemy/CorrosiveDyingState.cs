using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrosiveDyingState : CorrosiveState
{
    public AudioClip dieSound;

    public override void EnterState()
    {
        enemy.animator.SetTrigger("Die");

        enemy.audioSource.Stop();
        enemy.audioSource.PlayOneShot(dieSound);
    }
}
