using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SafeAreaComponent : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerComponent player;

        if (collision.TryGetComponent<PlayerComponent>(out player))
        {
            collision.GetComponent<PlayerComponent>().SafeAreasCount++;
            collision.GetComponent<LightDetectorComponent>().AddLightCount();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayerComponent player;

        if (collision.TryGetComponent<PlayerComponent>(out player))
        {
            collision.GetComponent<PlayerComponent>().SafeAreasCount--;
            collision.GetComponent<LightDetectorComponent>().ReduceLightCount();
        }
    }
}
