using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbusherState : MonoBehaviour, IState
{
    protected AmbusherEnemyComponent enemy;

    public void Awake()
    {
        enemy = GetComponentInParent<AmbusherEnemyComponent>();
    }

    public virtual void EnterState() { }

    public virtual void ExitState() { }

    public virtual void UpdateState() { }

    public virtual void FixedUpdateState() { }

    protected PlayerComponent GetPlayerInsideRange(float detectionDistance)
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, detectionDistance, LayerMask.GetMask("Player"));
        PlayerComponent playerComponent = player?.GetComponent<PlayerComponent>();

        return playerComponent;
    }
}
