using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyComponent : MonoBehaviour
{
    public SpriteRenderer sprite;
    public Rigidbody2D body;
    public SpriteMask spriteMask;
    public IState initialState;
    public IState currentState;
    protected bool deactivated = false;
    public PlayerComponent enemyTarget;

    private void Awake()
    {
        sprite = GetComponent<SpriteRenderer>();
        body = GetComponent<Rigidbody2D>();
        spriteMask = GetComponent<SpriteMask>();

        sprite.color = new Color(1, 1, 1, 0);
    }

    private void LateUpdate()
    {
        spriteMask.sprite = sprite.sprite;
    }

    public void ChangeState(IState newState)
    {
        if (deactivated) return;

        if (currentState != null) currentState.ExitState();

        currentState = newState;

        if (currentState != null) currentState.EnterState();
    }

    public void Spawn(Vector3 spawnPosition)
    {
        transform.position = spawnPosition;
        
        deactivated = false;

        gameObject.SetActive(true);
        sprite.DOFade(1f, 1f);

        ChangeState(initialState);
    }

    public void Deactivate(bool fade = true)
    {
        if (!gameObject.activeSelf) return;

        deactivated = true;

        if (fade)
        {
            sprite.DOFade(0f, 1f).OnComplete(() => gameObject.SetActive(false));
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public PlayerComponent GetPlayerInRange(float distance)
    {
        // Check if the player is in its detection range
        Collider2D player = Physics2D.OverlapCircle(transform.position, distance, LayerMask.GetMask("Player"));

        PlayerComponent playerComp;
        if (player == null || !player.TryGetComponent<PlayerComponent>(out playerComp) || playerComp.IsSafe()) return null;

        return playerComp;
    }

    public bool IsGameObjectInSight(GameObject obj, float maxDistance = -1)
    {
        if (obj == null) return false;

        Vector2 vectorToObject = obj.transform.position - transform.position;

        if (maxDistance > 0 && vectorToObject.magnitude > maxDistance) return false;

        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, vectorToObject.normalized, vectorToObject.magnitude);

        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger || hit.collider.gameObject == gameObject || hit.collider.gameObject == obj) continue;

            // Another object's collision is in the line of sight
            return false;
        }

        return true;
    }

    public Vector3 GetAvoidanceDirection(Vector3 targetDirection, float rayDistance)
    {
        Vector3 leftAvoid = Vector3.zero;
        Vector3 rightAvoid = Vector3.zero;
        List<GameObject> ignoreObjects = new List<GameObject> { gameObject, enemyTarget.gameObject };

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

    public float GetDistanceToTarget()
    {
        return enemyTarget != null ? Vector3.Distance(enemyTarget.transform.position, transform.position) : 0f;
    }

    public void MoveTo(Vector3 targetPosition, float movementSpeed, bool useObstacleAvoidance)
    {
        Vector3 targetDirection = (targetPosition - transform.position).normalized;

        // Obstacle detection
        if (useObstacleAvoidance)
        {
            Vector3 avoidVector = GetAvoidanceDirection(targetDirection, movementSpeed);
            targetDirection = (targetDirection + avoidVector).normalized;
        }

        body.MovePosition(transform.position + targetDirection * movementSpeed * Time.fixedDeltaTime);

        if (targetDirection.x > 0) FlipSprite(true);
        else if (targetDirection.x < 0) FlipSprite(false);
    }

    public void FlipSprite(bool fliped)
    {
        sprite.flipX = fliped;
    }
}
