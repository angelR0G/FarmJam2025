using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsanityAreaComponent : MonoBehaviour
{
    private const float EFFECT_TIME_INTERVAL = 0.2f;

    public float looseSanitySpeed = 1f;
    private SanityComponent affectedSanityComp = null;
    private float looseSanityTimer = 0f;
    private float sanityLostCounter = 0f;

    // Update is called once per frame
    void Update()
    {
        if (affectedSanityComp != null)
        {
            looseSanityTimer += Time.deltaTime;
            if (looseSanityTimer >= EFFECT_TIME_INTERVAL)
            {
                ReduceSanity();
                looseSanityTimer -= EFFECT_TIME_INTERVAL;
            }
        }
    }

    private void ReduceSanity()
    {
        sanityLostCounter += looseSanitySpeed * EFFECT_TIME_INTERVAL;
        if (sanityLostCounter >= 1f)
        {
            int sanityLost = Mathf.FloorToInt(sanityLostCounter);
            affectedSanityComp.LooseSanity(sanityLost);
            sanityLostCounter -= sanityLost;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SanityComponent enteringSanity;
        if (collision.TryGetComponent<SanityComponent>(out enteringSanity))
        {
            affectedSanityComp = enteringSanity;
            looseSanityTimer = 0;
            sanityLostCounter = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        SanityComponent exitingSanity;
        if (collision.TryGetComponent<SanityComponent>(out exitingSanity) && exitingSanity == affectedSanityComp)
        {
            affectedSanityComp = null;
        }
    }
}
