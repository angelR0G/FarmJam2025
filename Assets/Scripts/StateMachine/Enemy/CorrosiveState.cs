using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorrosiveState : MonoBehaviour, IState
{
    protected const float DETECTION_DISTANCE = 1f;
    protected CorrosiveEnemyComponent enemy;

    public void Awake()
    {
        enemy = GetComponentInParent<CorrosiveEnemyComponent>();
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
    protected void CheckPlayerInsideDetectionRange()
    {
        Collider2D player = Physics2D.OverlapCircle(transform.position, DETECTION_DISTANCE, LayerMask.GetMask("Player"));
        PlayerComponent playerComponent = player?.GetComponent<PlayerComponent>();

        if (player != null)
        {
            enemy.followTarget = player.gameObject;
            enemy.ChangeState(enemy.followState);
        }
    }

    protected void MoveTo(Vector3 targetPosition, float movementSpeed)
    {
        Vector3 targetDirection = (targetPosition - transform.position).normalized;

        // Obstacle detection
        Vector3 avoidVector = GetAvoidanceDirection(targetDirection, movementSpeed);
        targetDirection = (targetDirection + avoidVector).normalized;

        enemy.body.MovePosition(enemy.transform.position + targetDirection * movementSpeed * Time.fixedDeltaTime);

        if (targetDirection.x > 0) enemy.FlipSprite(true);
        else if (targetDirection.x < 0) enemy.FlipSprite(false);
    }

    protected float GetTargetDistance()
    {
        if (enemy.followTarget == null) return 0f;

        return Vector3.Distance(enemy.followTarget.transform.position, transform.position);
    }

    protected Vector3 GetAvoidanceDirection(Vector3 targetDirection, float rayDistance)
    {
        List<GameObject> ignoreObjects = new List<GameObject> { enemy.gameObject, enemy.followTarget };
        Vector3 leftAvoid = Vector3.zero;
        Vector3 rightAvoid = Vector3.zero;

        // Left raycast
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Quaternion.Euler(0, 0, 30f) * targetDirection, rayDistance);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger || ignoreObjects.Contains(hit.collider.gameObject)) continue;

            leftAvoid += Quaternion.Euler(0, 0, -90) * targetDirection * (1 - (hit.distance / rayDistance));
        }

        // Right raycast
        hits = Physics2D.RaycastAll(transform.position, Quaternion.Euler(0, 0, -30f) * targetDirection, rayDistance);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger || ignoreObjects.Contains(hit.collider.gameObject)) continue;

            rightAvoid += Quaternion.Euler(0, 0, 90) * targetDirection * (1 - (hit.distance / rayDistance));
        }

        return leftAvoid.sqrMagnitude >= rightAvoid.sqrMagnitude ? leftAvoid : rightAvoid;
    }
}
