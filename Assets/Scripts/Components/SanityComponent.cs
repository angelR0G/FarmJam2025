using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SanityComponent : MonoBehaviour
{
    [SerializeField]
    private int sanity = 0;
    [SerializeField]
    private int maxSanity = 100;

    [SerializeField]
    private int looseSanityRate = 5;
    [SerializeField]
    private int looseHealthRate = 5;

    private GameManager gameManager;
    private bool looseSanityState = false;
    private HealthComponent health;
    public bool insideLightSource = false;
    private float timer = 0f;


    // Start is called before the first frame update
    void Start()
    {
        sanity = maxSanity;
        gameManager = GameManager.Instance;
        gameManager.nightStart += OnNightStart;
        gameManager.nightEnd += OnNightEnd;

        health = GetComponentInParent<HealthComponent>();
    }

    public void LooseSanity(int s)
    {
        sanity -= s;
    }

    public void RestoreSanity(int s)
    {
        sanity = Math.Min(sanity + s, maxSanity);
    }

    public int GetSanity() { return sanity; }
    public float GetSanityPercentage() { return Math.Clamp((float)sanity / (float)maxSanity, 0, 1); }

    public void Update()
    {
        timer += Time.deltaTime;
        if (timer >= 1f)
        {
            timer = 0f;
            if (sanity <= 0)
            {
                health.LooseHealth(looseHealthRate);
            }
            if (!insideLightSource && looseSanityState && (sanity - looseSanityRate) >= 0)
            {
                LooseSanity(looseSanityRate);
            }
            if (!looseSanityState)
            {
                RestoreSanity(looseSanityRate);
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
}
