using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWateringState : PlayerState
{
    private const float WATERING_DISTANCE = 0.1f;
    private const float WATERING_AREA_SIZE = 0.2f;
    private const int MAX_LIQUID_AMOUNT = 1000;

    public int liquidAmount = 0;
    public bool isBlood = false;

    private CropComponent cropBeingWatered;
    private WaterSourceComponent waterSource;

    public override void EnterState()
    {
        GameObject obj = GetCompatibleObjectInFront();

        if (obj == null)
        {
            Debug.Log("~~ Aqui no hay nada para mojar ~~");
        }
        else if (obj.TryGetComponent<CropComponent>(out cropBeingWatered))
        {
            if (liquidAmount > 0)
            {
                cropBeingWatered.ChangeState(cropBeingWatered.wateredState);
                liquidAmount = 0;
                Debug.Log("~~ Agua pa ti, hermosa ~~");
            }
            else
            {
                cropBeingWatered = null;
                Debug.Log("~~ La regadera esta vacia. Tengo que llenarla ~~");
            }
        }
        else if (obj.TryGetComponent<WaterSourceComponent>(out waterSource))
        {
            liquidAmount = MAX_LIQUID_AMOUNT;
            isBlood = false;
            Debug.Log("~~ Aqui puedo llenar mi regadera ~~");
        }

        player.ChangeState(player.walkingState);
    }

    public override void ExitState()
    {
        cropBeingWatered = null;
        waterSource = null;
    }

    private GameObject GetCompatibleObjectInFront()
    {
        Vector2 wateringPosition = new Vector2(player.transform.position.x, player.transform.position.y) + player.facingDirection * WATERING_DISTANCE;
        Collider2D[] collisions = Physics2D.OverlapBoxAll(wateringPosition, new Vector2(WATERING_AREA_SIZE, WATERING_AREA_SIZE), 0);

        foreach (Collider2D c in collisions)
        {
            if (c.GetComponent<WaterSourceComponent>())
            {
                return c.gameObject;
            }

            CropComponent crop;
            if (c.gameObject.TryGetComponent<CropComponent>(out crop) && crop.currentState == crop.dryState)
            {
                return crop.gameObject;
            }
        }

        return null;
    }
}
