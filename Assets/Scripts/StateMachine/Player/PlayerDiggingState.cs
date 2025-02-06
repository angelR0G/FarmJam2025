using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDiggingState : PlayerState
{
    private const float DIGGING_DISTANCE = 0.1f;
    [SerializeField]
    private GameObject plotPrefab;

    public override void EnterState() 
    { 
        Vector3 diggingPosition;
        if (HasSpaceToDig(out diggingPosition))
        {
            GameObject newPlot = Instantiate(plotPrefab);
            newPlot.transform.position = diggingPosition;
        }

        player.ChangeState(player.walkingState);
    }

    private bool HasSpaceToDig(out Vector3 digPos)
    {
        MapComponent map = MapComponent.Instance;

        Vector2 diggingPosition = player.transform.position + new Vector3(player.facingDirection.x, player.facingDirection.y) * DIGGING_DISTANCE;
        digPos = diggingPosition;

        // Check if there is no object or collision in the tilemap blocking the ground
        if (!map.IsPositionFree(diggingPosition)) return false;

        diggingPosition = map.GetPositionAlignedToTileset(diggingPosition);

        // Check if there is an object there
        Collider2D[] collisions = Physics2D.OverlapBoxAll(diggingPosition, map.GetTilesetCellSize()/2, 0);
        foreach (Collider2D c in collisions)
        {
            // Does not take into account the player
            if (c.GetComponent<PlayerComponent>()) continue;

            return false;
        }

        // Update the position to be aligned with the tilemap
        digPos = diggingPosition;
        return true;
    }

    private bool IsOtherObjectBlockingPosition(Vector2 pos, Vector2 size)
    {
        // Check if there is an object there
        Collider2D[] collisions = Physics2D.OverlapBoxAll(pos, size, 0);
        foreach (Collider2D c in collisions)
        {
            // Does not take into account the player
            if (c.GetComponent<PlayerComponent>()) continue;

            return false;
        }

        return true;
    }
}
