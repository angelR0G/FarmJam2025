using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbusherAttackingState : AmbusherState
{
    private const float ATTACK_MOVEMENT_SPEED = 1.1f;
    private const float ATTACK_MAX_DISTANCE = 1.5f;
    private const float ATTACK_INITIAL_DISTANCE = 0.12f;

    private Vector3 attackDirection;

    public AudioClip attackSound;

    public override void EnterState()
    {
        attackDirection = (enemy.enemyTarget.transform.position - transform.position).normalized;

        // Updates attack values to start attacking in a new direction
        enemy.attackComponent.UpdateDamageArea(0.16f, 0.3f, Vector2.SignedAngle(Vector2.right, attackDirection));
        enemy.attackProjectile.transform.position = transform.position + attackDirection * ATTACK_INITIAL_DISTANCE;

        enemy.animator.SetTrigger("Attack");

        enemy.audioSource.clip = attackSound;
        enemy.audioSource.Play();
    }

    public override void ExitState()
    {
        SetProjectileActive(false);
    }

    public override void FixedUpdateState()
    {
        if (enemy.attackProjectile.transform.localPosition.magnitude >= ATTACK_MAX_DISTANCE)
        {
            enemy.ChangeState(enemy.idleState);
        }
        else
        {
            UpdateProjectilePosition();
        }
    }

    private void UpdateProjectilePosition()
    {
        enemy.attackProjectile.transform.position += attackDirection * ATTACK_MOVEMENT_SPEED * Time.fixedDeltaTime;
    }

    public void SetProjectileActive(bool newState)
    {
        enemy.attackComponent.SetDamageAreaActive(newState);
        enemy.attackComponent.sprite.enabled = newState;
        enemy.attackProjectile.SetActive(newState);
    }
}
