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

    protected Vector3 GetAvoidanceDirection(Vector3 targetDirection, float rayDistance)
    {
        Vector3 leftAvoid = Vector3.zero;
        Vector3 rightAvoid = Vector3.zero;
        List<GameObject> ignoredObjects = new List<GameObject> {enemy.gameObject, enemy.attackTarget};

        // Left raycast
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Quaternion.Euler(0, 0, 30f) * targetDirection, rayDistance);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger || ignoredObjects.Contains(hit.collider.gameObject)) continue;

            leftAvoid += Quaternion.Euler(0, 0, -90) * targetDirection * 1f * (1 - (hit.distance / rayDistance));
        }

        // Right raycast
        hits = Physics2D.RaycastAll(transform.position, Quaternion.Euler(0, 0, -30f) * targetDirection, rayDistance);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger || ignoredObjects.Contains(hit.collider.gameObject)) continue;

            rightAvoid += Quaternion.Euler(0, 0, 90) * targetDirection * 1f * (1 - (hit.distance / rayDistance));
        }

        return leftAvoid.sqrMagnitude >= rightAvoid.sqrMagnitude ? leftAvoid : rightAvoid;
    }

    protected void MoveTo(Vector3 direction, float speed)
    {
        // Obstacle detection
        Vector3 avoidVector = GetAvoidanceDirection(direction, speed);
        direction = (direction + avoidVector).normalized;

        enemy.body.MovePosition(enemy.transform.position + direction * speed * Time.fixedDeltaTime);

        if (direction.x > 0) enemy.FlipSprite(true);
        else if (direction.x < 0) enemy.FlipSprite(false);
    }

    protected bool IsTargetInLight()
    {
        SanityComponent targetSanityComponent;

        if (enemy.attackTarget == null || !enemy.attackTarget.TryGetComponent<SanityComponent>(out targetSanityComponent))
            return true;

        return targetSanityComponent.IsInsideLight();
    }

    protected float GetDistanceToTarget()
    {
        return enemy.attackTarget != null ? Vector3.Distance(enemy.attackTarget.transform.position, transform.position) : 0f;
    }
}
