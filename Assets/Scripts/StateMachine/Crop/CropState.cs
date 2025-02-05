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

    public void EnterState() { }

    public void ExitState() { }

    public void UpdateState() { }

    public void FixedUpdateState() { }
}
