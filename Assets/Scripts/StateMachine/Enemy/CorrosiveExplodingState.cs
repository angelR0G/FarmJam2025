using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrosiveExplodingState : CorrosiveState
{
    private const float EPXLOSION_RANGE = 0.65f;
    public int explosionDamage = 55;

    private bool playerHit = false;

    public AudioClip explosionSound;

    public override void EnterState()
    {
        // Prevents dying during the animation
        enemy.healthComp.RestoreHealth(100);

        enemy.enemyCollider.enabled = false;

        playerHit = false;
        enemy.animator.SetTrigger("Explode");

        enemy.audioSource.clip = explosionSound;
        enemy.audioSource.Play();
    }

    public override void ExitState()
    {
        enemy.enemyCollider.enabled = true;
    }

    public override void FixedUpdateState()
    {
        if (playerHit) return;

        PlayerComponent player = enemy.GetPlayerInRange(EPXLOSION_RANGE);
        if (player != null)
        {
            player.healthComponent.LooseHealth(explosionDamage);
            playerHit = true;
        }
    }
}
