using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerDieState : StalkerState
{
    public AudioClip dieSound;

    public override void EnterState()
    {
        enemy.animator.SetTrigger("Die");

        enemy.audioSource.clip = dieSound;
        enemy.audioSource.Play();
    }
}
