using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWateringState : PlayerState
{
    private const float WATERING_DISTANCE = 0.1f;
    private const float WATERING_AREA_SIZE = 0.2f;

    public override void EnterState()
    {
        CropComponent crop = GetCropInFront();

        if (crop != null)
        {
            crop.ChangeState(crop.wateredState);
            Debug.Log("~~ Agua pa ti, hermosa ~~");
        }
        else
        {
            Debug.Log("~~ Aqui no hay nada para mojar ~~");
        }

        player.ChangeState(player.walkingState);
    }

    private CropComponent GetCropInFront()
    {
        Vector2 wateringPosition = new Vector2(player.transform.position.x, player.transform.position.y) + player.facingDirection * WATERING_DISTANCE;
        Collider2D[] collisions = Physics2D.OverlapBoxAll(wateringPosition, new Vector2(WATERING_AREA_SIZE, WATERING_AREA_SIZE), 0);

        foreach (Collider2D c in collisions)
        {
            CropComponent crop;
            if (c.gameObject.TryGetComponent<CropComponent>(out crop) && crop.currentState == crop.dryState)
            {
                return crop;
            }
        }

        return null;
    }
}
