using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StalkerChaseState : StalkerState
{
    private const float ATTACK_DISTANCE = 1f;
    private const float CHASE_MAX_DISTANCE = 3f;
    private const float MAX_SPEED = 1.5f;
    private const float ACCELERATION = 0.75f;

    public GameObject target;
    private bool colisionAvoidanceEnabled = false;
    private float speed;

    public override void EnterState()
    {
        colisionAvoidanceEnabled = true;
        speed = 0.0f;
    }

    public override void ExitState()
    {
        colisionAvoidanceEnabled = false;
    }

    public override void FixedUpdateState()
    {
        Vector3 vectorToTarget = target.transform.position - transform.position;
        float distanceToTarget = vectorToTarget.magnitude;

        if (distanceToTarget < ATTACK_DISTANCE)
        {
            enemy.ChangeState(enemy.attackState);
        }
        else if (distanceToTarget < CHASE_MAX_DISTANCE)
        {
            MoveTo(vectorToTarget.normalized);
        }
        else
        {
            enemy.ChangeState(enemy.wanderState);
        }
    }

    private void MoveTo(Vector3 direction)
    {
        if (speed < MAX_SPEED)
        {
            speed = Mathf.Min(speed + ACCELERATION * Time.fixedDeltaTime, MAX_SPEED);
        }

        enemy.body.MovePosition(enemy.transform.position + direction * speed * Time.fixedDeltaTime);
    }
}
