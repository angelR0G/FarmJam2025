using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerState : MonoBehaviour, IState
{
    protected StalkerEnemyComponent enemy;

    public void Awake()
    {
        enemy = GetComponentInParent<StalkerEnemyComponent>();
    }

    public virtual void EnterState() { }

    public virtual void ExitState() { }

    public virtual void UpdateState() { }

    public virtual void FixedUpdateState() { }
}
