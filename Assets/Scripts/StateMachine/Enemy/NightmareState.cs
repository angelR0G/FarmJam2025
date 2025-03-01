using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightmareState : MonoBehaviour, IState
{
    protected NightmareEnemyComponent enemy;

    public void Awake()
    {
        enemy = GetComponentInParent<NightmareEnemyComponent>();
    }

    public virtual void EnterState() { }

    public virtual void ExitState() { }

    public virtual void UpdateState() { }

    public virtual void FixedUpdateState() { }

    protected bool IsTargetInLight()
    {
        LightDetectorComponent targetLightDetector;

        if (enemy.enemyTarget == null || !enemy.enemyTarget.TryGetComponent<LightDetectorComponent>(out targetLightDetector))
            return true;

        return targetLightDetector.IsInsideLight();
    }
}
