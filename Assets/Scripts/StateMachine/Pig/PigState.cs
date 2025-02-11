using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigState : MonoBehaviour, IState
{
    protected PigComponent pig;

    public void Awake()
    {
        pig = GetComponentInParent<PigComponent>();
    }

    public virtual void EnterState() { }

    public virtual void ExitState() { }

    public virtual void UpdateState() { }

    public virtual void FixedUpdateState() { }
}
