using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWateringState : PlayerState
{
    private const float WATERING_DISTANCE = 0.1f;
    private const float WATERING_AREA_SIZE = 0.2f;
    private const int MAX_LIQUID_AMOUNT = 1000;
    private const float WATERING_SPEED = 100f;
    private const float BLOOD_WATERING_SPEED = 400f;

    private int liquidAmount = 0;
    public bool isBlood = false;

    public AudioClip wateringSound;
    public AudioClip fillingWaterCanSound;

    public int LiquidAmount { get { return liquidAmount; } private set { liquidAmount = value; } }

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
                    DialogueSystem.Instance.DisplayDialogue(new Dialogue("The watering can is empty. I need to fill it."));
                    player.ChangeState(player.walkingState);
                }
                else if (!IsLiquidCompatibleWithCrop(cropBeingWatered))
                {
                    cropBeingWatered = null;
                    DialogueSystem.Instance.DisplayDialogue(new Dialogue("I can't water this plant with this."));
                    player.ChangeState(player.walkingState);
                }
                else
                {
                    player.animator.SetTrigger("Water");
                    player.animator.SetBool("LoopWater", true);
                    player.IsInteractionEnabled = false;

                    player.audioSource.clip = wateringSound;
                    player.audioSource.loop = true;
                    player.audioSource.Play();
                }
            }
            else 
            {
                waterSource = obj.GetComponent<WaterSourceComponent>();
                player.animator.SetTrigger("Water");
                player.animator.SetBool("LoopWater", false);
                player.IsInteractionEnabled = false;

                player.audioSource.clip = fillingWaterCanSound;
                player.audioSource.loop = true;
                player.audioSource.Play();
            }
        }
        else
        {
            DialogueSystem.Instance.DisplayDialogue(new Dialogue("Nothing to water here.", -1));
            player.ChangeState(player.walkingState);
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
        float watertingSpeed = isBlood ? BLOOD_WATERING_SPEED : WATERING_SPEED;
        int waterLimit = Math.Min(dryCrop.GetRemainingWaterForGrowth(), liquidAmount);
        int waterAmount = Math.Min(Mathf.CeilToInt(watertingSpeed * Time.deltaTime), waterLimit);

        liquidAmount -= waterAmount;
        dryCrop.Water(waterAmount);
    }

    private void FillWaterCan()
    {
        if (isBlood)
        {
            liquidAmount = 0;
            isBlood = false;
        }

        liquidAmount += Mathf.CeilToInt(WATERING_SPEED * Time.deltaTime);

        if (liquidAmount >= MAX_LIQUID_AMOUNT)
        {
            liquidAmount = MAX_LIQUID_AMOUNT;
            waterSource = null;
        }
    }

    public void AddBlood(int bloodAmount)
    {
        if (isBlood)
        {
            liquidAmount = Math.Min(liquidAmount + bloodAmount, MAX_LIQUID_AMOUNT);
        }
        else
        {
            liquidAmount = Math.Min(bloodAmount, MAX_LIQUID_AMOUNT);
            isBlood = true;
        }
    }

    public override void ExitState()
    {
        cropBeingWatered = null;
        waterSource = null;

        player.audioSource.loop = false;
        player.audioSource.Stop();
    }

    private bool IsLiquidCompatibleWithCrop(CropComponent crop)
    {
        // Evil crops require blood while normal plants require water
        return crop.dryState.isWateredWithBlood == isBlood;
    }

    // Returns the object that can be interacted with the water can (crop, water source or null)
    private GameObject GetCompatibleObjectInFront()
    {
        Vector2 wateringPosition = new Vector2(player.transform.position.x, player.transform.position.y) + player.facingDirection * WATERING_DISTANCE;
        Collider2D[] collisions = Physics2D.OverlapBoxAll(wateringPosition, new Vector2(WATERING_AREA_SIZE, WATERING_AREA_SIZE), 0);
        CropComponent crop;

        GameObject closestValidGameObject = null;
        float distanceToClosestGameObject = 0;

        foreach (Collider2D c in collisions)
        {
            // Water sources to fill the water
            if (c.GetComponent<WaterSourceComponent>())
            {
                float distance = GetSqrDistanceToObject(c.gameObject);

                if (closestValidGameObject == null || (liquidAmount < MAX_LIQUID_AMOUNT && distance < distanceToClosestGameObject))
                {
                    closestValidGameObject = c.gameObject;
                    distanceToClosestGameObject = distance;
                }
            }

            // Crops that can be watered
            else if (c.gameObject.TryGetComponent<CropComponent>(out crop) && crop.currentState == crop.dryState)
            {
                float distance = GetSqrDistanceToObject(c.gameObject);

                if (closestValidGameObject == null || distance < distanceToClosestGameObject)
                {
                    closestValidGameObject = c.gameObject;
                    distanceToClosestGameObject = distance;
                }
            }
        }

        return closestValidGameObject;
    }

    private float GetSqrDistanceToObject(GameObject obj)
    {
        return Vector3.SqrMagnitude(obj.transform.position - player.transform.position);
    }

    public float GetFillPorcentage()
    {
        return Math.Clamp((float)liquidAmount / (float)MAX_LIQUID_AMOUNT, 0, 1);
    }
}
