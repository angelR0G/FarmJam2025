using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightmareTeleportState : NightmareState
{
    private const float MAX_DISTANCE_FROM_TARGET = 1f;

    public AudioClip teleportSound;

    public override void EnterState()
    {
        enemy.transform.position = GetTeleportPosition();
        enemy.animator.SetTrigger("Teleport");

        enemy.SetWingSoundEnabled(false);
        enemy.audioSource.clip = teleportSound;
        enemy.audioSource.Play();
    }

    public override void UpdateState()
    {
        if (enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1)
        {
            enemy.ChangeState(enemy.flyingState);
        }
    }

    private Vector3 GetTeleportPosition()
    {
        List<GameObject> ignoredObjects = new List<GameObject> { enemy.gameObject, enemy.attackTarget };
        Vector2 teleportDirection = Vector2.right;
        
        // Cast to player to get the movement direction
        PlayerComponent player;
        if (enemy.attackTarget.TryGetComponent<PlayerComponent>(out player))
        {
            teleportDirection = enemy.attackTarget.GetComponent<PlayerComponent>().facingDirection;
        }

        // Do a raycast to find the closest collision that blocks teleport
        float teleportDistance = MAX_DISTANCE_FROM_TARGET;
        RaycastHit2D[] hits = Physics2D.RaycastAll(enemy.attackTarget.transform.position, teleportDirection, MAX_DISTANCE_FROM_TARGET);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.isTrigger || ignoredObjects.Contains(hit.collider.gameObject)) continue;

            if (teleportDistance > hit.distance)
                teleportDistance = hit.distance;
        }

        // Calculate the final teleport position
        Vector3 teleportOffset = new Vector3(teleportDirection.x, teleportDirection.y) * (teleportDistance - enemy.enemyCollider.radius);
        return enemy.attackTarget.transform.position + teleportOffset;
    }
}
