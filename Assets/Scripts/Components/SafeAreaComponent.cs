using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaComponent : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerComponent player;
        DaylightDestroyComponent daylightDestroyable;

        if (collision.TryGetComponent<PlayerComponent>(out player))
        {
            collision.GetComponent<PlayerComponent>().SafeAreasCount++;
            collision.GetComponent<LightDetectorComponent>().AddLightCount();
        }
        else if (collision.TryGetComponent<DaylightDestroyComponent>(out daylightDestroyable))
        {
            daylightDestroyable.SafeAreasCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerComponent player;
        DaylightDestroyComponent daylightDestroyable;

        if (collision.TryGetComponent<PlayerComponent>(out player))
        {
            collision.GetComponent<PlayerComponent>().SafeAreasCount--;
            collision.GetComponent<LightDetectorComponent>().ReduceLightCount();
        }
        else if (collision.TryGetComponent<DaylightDestroyComponent>(out daylightDestroyable))
        {
            daylightDestroyable.SafeAreasCount--;
        }
    }
}
