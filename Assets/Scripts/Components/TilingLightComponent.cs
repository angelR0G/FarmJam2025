using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilingLightComponent : LightSourceComponent
{
    private float baseRadius = 1f;
    private float targetRadius = 1f;
    private float flickeringSpeed = 1f;
    private float baseIntensity = 1f;
    public float minFlickeringSpeed = 0.5f;
    public float maxFlickeringSpeed = 1f;
    public float maxRadiusOffset = 0.2f;
    public float maxIntensityOffset = 0.5f;


    protected override void Start()
    {
        base.Start();

        baseRadius = light2D.pointLightOuterRadius;
        baseIntensity = light2D.intensity;

        if (minFlickeringSpeed > maxFlickeringSpeed) minFlickeringSpeed = maxFlickeringSpeed;

        CalculateNewFlickering();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLightRadius();
        UpdateLightIntensity();
    }

    private void CalculateNewFlickering()
    {
        targetRadius = baseRadius + Random.Range(-maxRadiusOffset, maxRadiusOffset);
        flickeringSpeed = Random.Range(minFlickeringSpeed, maxFlickeringSpeed);
    }

    private void UpdateLightRadius()
    {
        float currentRadius = light2D.pointLightOuterRadius;
        float speed = targetRadius < currentRadius ? -flickeringSpeed : flickeringSpeed;

        float newRadius = currentRadius + speed * Time.deltaTime;
        light2D.pointLightOuterRadius = newRadius;

        // Check if target radius is passed
        if ((speed < 0 && newRadius <= targetRadius) || (speed > 0 && newRadius >= targetRadius))
            CalculateNewFlickering();
    }

    private void UpdateLightIntensity()
    {
        float offsetScale = light2D.pointLightOuterRadius / baseRadius - 1;

        light2D.intensity = baseIntensity + maxIntensityOffset * offsetScale;
    }
}
