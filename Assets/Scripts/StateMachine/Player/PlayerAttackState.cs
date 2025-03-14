using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerState
{
    private const float COOLDOWN_TIME = 0.18f;
    private const float IMPULSE_SPEED = 0.5f;
    private const float ATTACK_RANGE = 0.25f;
    private const float ATTACK_WIDTH = 0.25f;
    private const float MIN_TIME_REQUIRED_FOR_COMBO = 0.06f;

    private float cooldown = 0;
    private bool isOnCooldown = false;
    private bool comboActivated = false;
    private float comboTimer = 1f;
    public int attackDamage = 10;
    public AudioClip attackSound = null;

    public override void EnterState()
    {
        player.IsInteractionEnabled = false;
        player.onAnimEvent = Attack;
        player.onAnimFinished = StartCooldown;

        player.attackComponent.UpdateDamageArea(ATTACK_RANGE, ATTACK_WIDTH, Vector2.SignedAngle(Vector2.right, player.facingDirection));
        player.attackComponent.damage = attackDamage;
        player.attackComponent.ConfigureSprite(new Vector3(0.04f, 0), false, player.animator.GetInteger("Direction") == 3);
        player.attackComponent.PlayAnimation("Attack");
        player.audioSource.PlayOneShot(attackSound);

        isOnCooldown = false;
        comboActivated = false;
        comboTimer = 0f;

        player.animator.SetTrigger("Attack");
    }

    public override void ExitState()
    {
        player.attackComponent.SetDamageAreaActive(false);
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
        CheckComboActivated();

        if (isOnCooldown)
        {
            if (comboActivated)
            {
                player.ChangeState(player.finishAttackState);
            }
            else 
            {
                cooldown -= Time.deltaTime;
                if (cooldown <= 0)
                    player.ChangeState(player.walkingState);
            }
        }
    }

    public void CheckComboActivated()
    {
        // Check that a minimum amount of time passed so that input can refresh
        if (comboTimer >= MIN_TIME_REQUIRED_FOR_COMBO)
        {
            if (player.inputComponent.interact)
                comboActivated = true;
        }
        else
        {
            comboTimer += Time.deltaTime;
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
