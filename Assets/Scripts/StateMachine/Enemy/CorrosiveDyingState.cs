using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrosiveDyingState : CorrosiveState
{
    public override void EnterState()
    {
        enemy.animator.SetTrigger("Die");
    }
}
