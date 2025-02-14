using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWateringState : PlayerState
{
    private const float WATERING_DISTANCE = 0.1f;
    private const float WATERING_AREA_SIZE = 0.2f;
    private const int MAX_LIQUID_AMOUNT = 1000;
    private const float WATERING_SPEED = 300f;

    public int liquidAmount = 0;
    public bool isBlood = false;

    private CropComponent cropBeingWatered;
    private WaterSourceComponent waterSource;

    public override void EnterState()
    {
        GameObject obj = GetCompatibleObjectInFront();

        if (obj != null) {
            if (obj.TryGetComponent<CropComponent>(out cropBeingWatered))
            {
                if (liquidAmount <= 0)
                {
                    cropBeingWatered = null;
                    player.ChangeState(player.walkingState);
                    Debug.Log("~~ La regadera esta vacia. Tengo que llenarla ~~");
                }
                else
                {
                    player.animator.SetTrigger("Water");
                    player.animator.SetBool("LoopWater", true);
                    player.IsInteractionEnabled = false;
                }
            }
            else 
            {
                waterSource = obj.GetComponent<WaterSourceComponent>();
                player.animator.SetTrigger("Water");
                player.animator.SetBool("LoopWater", false);
                player.IsInteractionEnabled = false;
            }
        }
        else
        {
            player.ChangeState(player.walkingState);
            Debug.Log("~~ Aqui no hay nada para mojar ~~");
        }
    }

    public override void UpdateState()
    {
        if (cropBeingWatered != null)
            WaterCrop();
        else if (waterSource != null)
            FillWaterCan();
        else
            player.ChangeState(player.walkingState);
    }

    private void WaterCrop()
    {
        if (cropBeingWatered.currentState != cropBeingWatered.dryState || liquidAmount <= 0)
        {
            cropBeingWatered = null;
            return;
        }

        CropDryState dryCrop = cropBeingWatered.dryState;
        int waterLimit = Math.Min(dryCrop.GetRemainingWaterForGrowth(), liquidAmount);
        int waterAmount = Math.Min(Mathf.CeilToInt(WATERING_SPEED * Time.deltaTime), waterLimit);

        liquidAmount -= waterAmount;
        dryCrop.Water(waterAmount);
    }

    private void FillWaterCan()
    {
        liquidAmount += Mathf.CeilToInt(WATERING_SPEED * Time.deltaTime);
        isBlood = false;

        if (liquidAmount >= MAX_LIQUID_AMOUNT)
        {
            liquidAmount = MAX_LIQUID_AMOUNT;
            waterSource = null;
        }
    }

    public override void ExitState()
    {
        cropBeingWatered = null;
        waterSource = null;
    }

    // Returns the object that can be interacted with the water can (crop, water source or null)
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
