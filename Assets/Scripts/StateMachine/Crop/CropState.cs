using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropState : MonoBehaviour, IState
{
    protected CropComponent crop;

    void Awake()
    {
        crop = GetComponentInParent<CropComponent>();
    }

    public virtual void EnterState() { }

    public virtual void ExitState() { }

    public virtual void UpdateState() { }

    public virtual void FixedUpdateState() { }
}
