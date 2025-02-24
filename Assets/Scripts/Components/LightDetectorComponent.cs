using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LightDetectorComponent : MonoBehaviour
{
    private int lightSourcesCount = 0;
    public UnityEvent enterLight;
    public UnityEvent exitLight;

    public bool IsInsideLight()
    {
        return lightSourcesCount > 0;
    }

    public void AddLightCount()
    {
        lightSourcesCount++;
        if (lightSourcesCount == 1)
        {
            enterLight.Invoke();
        }
    }

    public void ReduceLightCount()
    {
        lightSourcesCount--;
        if (lightSourcesCount == 0)
        {
            exitLight.Invoke();
        }
    }
}
