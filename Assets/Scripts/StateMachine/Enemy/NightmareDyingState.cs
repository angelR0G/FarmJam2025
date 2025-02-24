using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightmareDyingState : NightmareState
{
    public override void EnterState()
    {
        enemy.animator.SetTrigger("Die");
    }
}
