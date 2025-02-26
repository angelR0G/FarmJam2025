using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class SanityComponent : MonoBehaviour
{
    [SerializeField]
    private int sanity = 0;
    [SerializeField]
    private int maxSanity = 100;
    [SerializeField]
    private float looseSanityRate = 5;
    [SerializeField]
    private float darknessTimeBeforeLoosingSanity = 3f;

    public UnityEvent onInsane;

    private GameManager gameManager;
    private bool looseSanityState = false;
    private float darknessTimer = 0f;
    private float sanityLost = 0f;
    LightDetectorComponent lightDetector;


    // Start is called before the first frame update
    void Start()
    {
        sanity = maxSanity;
        gameManager = GameManager.Instance;
        gameManager.nightStart += OnNightStart;
        gameManager.nightEnd += OnNightEnd;

        lightDetector = GetComponent<LightDetectorComponent>();
        lightDetector.enterLight.AddListener(ResetDarknessTimer);
    }

    public void LooseSanity(int s)
    {
        if (sanity - s <= 0)
        {
            if (sanity > 0) onInsane.Invoke();

            sanity = 0;
        }
        else
        {
            sanity -= s;
        }
    }

    public void RestoreSanity(int s)
    {
        sanity = Math.Min(sanity + s, maxSanity);
    }

    public int GetSanity() { return sanity; }
    public float GetSanityPercentage() { return Math.Clamp((float)sanity / (float)maxSanity, 0, 1); }

    public void Update()
    {
        if (looseSanityState && !lightDetector.IsInsideLight())
        {
            if (darknessTimer < darknessTimeBeforeLoosingSanity)
            {
                darknessTimer += Time.deltaTime;
            }
            else
            {
                sanityLost += Time.deltaTime * looseSanityRate;
                
                if (sanityLost >= 1f)
                {
                    int roundedSanityLost = Mathf.FloorToInt(sanityLost);
                    LooseSanity(roundedSanityLost);
                    sanityLost -= roundedSanityLost;
                }
            }
        }
    }

    private void OnNightStart(object sender, int hour)
    {
        looseSanityState = true;
    }

    private void OnNightEnd(object sender, int hour)
    {
        looseSanityState = false;
    }

    private void ResetDarknessTimer()
    {
        darknessTimer = 0f;
        sanityLost = 0f;
    }

    public bool IsInsane()
    {
        return sanity <= 0;
    }
}
