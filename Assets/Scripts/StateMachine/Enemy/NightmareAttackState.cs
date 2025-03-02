using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightmareAttackState : NightmareState
{
    private const float ATTACK_RANGE = 0.35f;
    private const float ATTACK_WIDTH = 0.2f;
    private const float ATTACK_TIME = 0.6f;
    private const float ATTACK_TIME_IN_LIGHT = 1.2f;

    private float attackCooldown = ATTACK_TIME;

    public AudioClip attackSound;

    public override void EnterState()
    {
        enemy.animator.SetTrigger("Attack");
        attackCooldown = enemy.lightDetector.IsInsideLight() ? ATTACK_TIME_IN_LIGHT : ATTACK_TIME;

        enemy.SetWingSoundEnabled(false);
        enemy.audioSource.clip = attackSound;
        enemy.audioSource.Play();
    }

    public override void ExitState()
    {
        enemy.attackComponent.SetDamageAreaActive(false);
    }

    public override void UpdateState()
    {
        attackCooldown -= Time.deltaTime;

        if (attackCooldown <= 0)
            enemy.ChangeState(enemy.flyingState);
    }

    public void Attack()
    {
        Vector2 attackDirection = enemy.enemyTarget.transform.position - transform.position;
        enemy.attackComponent.UpdateDamageArea(ATTACK_RANGE, ATTACK_WIDTH, Vector2.SignedAngle(Vector2.right, attackDirection));
        enemy.attackComponent.SetDamageAreaActive(true);
    }
}
