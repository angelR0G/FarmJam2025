using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDiggingState : PlayerState
{
    private const float DIGGING_DISTANCE = 0.1f;
    [SerializeField]
    private GameObject plotPrefab;
    private bool canDig = false;
    private Vector3 diggingPosition;

    public override void EnterState() 
    {
        canDig = HasSpaceToDig();

        if (!canDig)
        {
            player.ChangeState(player.walkingState);
            Debug.Log("~~ No puedo cavar aqui ~~");
        }
    }

    public override void UpdateState()
    {
        if (canDig)
        {
            player.animator.SetTrigger("Dig");
            player.onAnimFinished = OnAnimationFinished;
            canDig = false;
        }
    }

    public void OnAnimationFinished()
    {
        // Creates the plot to plant crops
        GameObject newPlot = Instantiate(plotPrefab);
        newPlot.transform.position = diggingPosition;

        // Remove callback
        player.onAnimFinished = null;

        player.ChangeState(player.walkingState);
    }

    private bool HasSpaceToDig()
    {
        MapComponent map = MapComponent.Instance;

        Vector3 digPos = player.transform.position + new Vector3(player.facingDirection.x, player.facingDirection.y) * DIGGING_DISTANCE;

        // Check if there is no object or collision in the tilemap blocking the ground
        if (!map.IsPositionFree(digPos)) return false;

        digPos = map.GetPositionAlignedToTileset(digPos);

        // Check if there is an object there
        Collider2D[] collisions = Physics2D.OverlapBoxAll(digPos, map.GetTilesetCellSize()/2, 0);
        foreach (Collider2D c in collisions)
        {
            // Does not take into account the player
            if (c.GetComponent<PlayerComponent>()) continue;

            return false;
        }

        // Save the position aligned with the tilemap
        diggingPosition = digPos;
        return true;
    }
}
