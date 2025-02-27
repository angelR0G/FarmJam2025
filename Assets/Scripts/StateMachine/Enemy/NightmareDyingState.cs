using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightmareDyingState : NightmareState
{
    public AudioClip dieSound;

    public override void EnterState()
    {
        enemy.animator.SetTrigger("Die");

        enemy.SetWingSoundEnabled(false);
        enemy.audioSource.clip = dieSound;
        enemy.audioSource.Play();
    }
}
