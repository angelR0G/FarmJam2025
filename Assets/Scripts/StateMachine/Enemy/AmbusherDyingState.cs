using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbusherDyingState : AmbusherState
{
    public AudioClip dieSound;

    public override void EnterState()
    {
        enemy.animator.SetTrigger("Die");
        enemy.audioSource.PlayOneShot(dieSound);
    }
}
