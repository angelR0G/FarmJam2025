using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFinishAttackState : PlayerState
{
    private const float COOLDOWN_TIME = 0.2f;
    private const float IMPULSE_SPEED = 1f;
    private const float ATTACK_RANGE = 0.4f;
    private const float ATTACK_WIDTH = 0.1f;

    private float cooldown = 0;
    private bool isOnCooldown = false;
    public int attackDamage = 20;

    public override void EnterState()
    {
        player.IsInteractionEnabled = false;
        player.onAnimEvent = Attack;
        player.onAnimFinished = StartCooldown;

        player.attackComponent.UpdateDamageArea(ATTACK_RANGE, ATTACK_WIDTH, Vector2.SignedAngle(Vector2.right, player.facingDirection));
        player.attackComponent.damage = attackDamage;
        isOnCooldown = false;

        player.animator.SetTrigger("FinishAttack");
    }

    public override void ExitState()
    {
        player.IsInteractionEnabled = true;
        player.onAnimEvent = null;
        player.onAnimFinished = null;
    }

    public override void FixedUpdateState()
    {
        MoveForward();
    }

    public override void UpdateState()
    {
        if (isOnCooldown)
        {
            cooldown -= Time.deltaTime;
            if (cooldown <= 0)
                player.ChangeState(player.walkingState);
        }
    }

    public void Attack()
    {
        player.attackComponent.SetDamageAreaActive(true);
    }

    public void StartCooldown()
    {
        player.attackComponent.SetDamageAreaActive(false);

        isOnCooldown = true;
        cooldown = COOLDOWN_TIME;
    }

    private void MoveForward()
    {
        if (isOnCooldown) return;

        Vector3 attackDirection = new Vector3(player.facingDirection.x, player.facingDirection.y);
        float speed = (1 - (player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime) % 1f) * IMPULSE_SPEED;

        player.body.MovePosition(player.transform.position + attackDirection * speed * Time.fixedDeltaTime);
    }
}
