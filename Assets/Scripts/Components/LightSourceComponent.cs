using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering.Universal;

public class LightSourceComponent : MonoBehaviour
{
    protected Light2D light2D;
    protected CircleCollider2D lightTrigger;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        light2D = GetComponent<Light2D>();
        lightTrigger = GetComponent<CircleCollider2D>();

        light2D.enabled = true;
        lightTrigger.enabled = true;
    }

    public void SetLightEnabled(bool newState)
    {
        light2D.enabled = newState;
        lightTrigger.enabled = newState;
    }

    public void ToggleLightEnabled()
    {
        light2D.enabled = !light2D.enabled;
        lightTrigger.enabled = !lightTrigger.enabled;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SanityComponent sanity;
        LightDetectorComponent lightDetector;

        if (other.TryGetComponent<SanityComponent>(out sanity))
        {
            sanity.lightSourcesCount++;
        }
        else if (other.TryGetComponent<LightDetectorComponent>(out lightDetector))
        {
            lightDetector.AddLightCount();
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        SanityComponent sanity;
        LightDetectorComponent lightDetector;

        if (other.TryGetComponent<SanityComponent>(out sanity))
        {
            sanity.lightSourcesCount--;
        }
        else if (other.TryGetComponent<LightDetectorComponent>(out lightDetector))
        {
            lightDetector.ReduceLightCount();
        }
    }
}
