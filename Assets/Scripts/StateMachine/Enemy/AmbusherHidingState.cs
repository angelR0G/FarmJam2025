using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbusherHidingState : AmbusherState
{
    private const float ACTIVATION_RANGE = 0.5f;

    public override void EnterState()
    {
        if (enemy.hidden)
        {
            enemy.sprite.enabled = false;
        }
        else
        {
            enemy.animator.SetTrigger("Hide");
        }
    }

    public override void UpdateState()
    {
        if (enemy.hidden == false && enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            enemy.sprite.enabled = false;
            enemy.hidden = true;
        }
    }

    public override void FixedUpdateState()
    {
        PlayerComponent player = GetPlayerInsideRange(ACTIVATION_RANGE);

        if (player != null)
        {
            enemy.attackTarget = player.gameObject;
            enemy.ChangeState(enemy.idleState);
        }
    }
}
