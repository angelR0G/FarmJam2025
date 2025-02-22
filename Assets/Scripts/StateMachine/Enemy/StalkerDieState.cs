using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerDieState : StalkerState
{
    public override void EnterState()
    {
        enemy.animator.SetTrigger("Die");
    }
}
