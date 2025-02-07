using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public event EventHandler<TimeSpan> worldTimeChanged;
    public event EventHandler<int> dayChanged;
    public event EventHandler<int> hourChanged;

    public const int MinutesInDay = 1440;

    [SerializeField]
    private float dayLength;

    private TimeSpan currentTime;
    private float minuteLenght => dayLength / MinutesInDay;
    [SerializeField]
    private Gradient gradient;

    private Light2D light;

    [SerializeField]
    private int numDays;

    private void Awake()
    {
        light = GetComponent<Light2D>();
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(AddMinute());
    }
    private IEnumerator AddMinute()
    {
        currentTime += TimeSpan.FromMinutes(1);
        worldTimeChanged?.Invoke(this, currentTime);
        light.color = gradient.Evaluate(PercentOfDay());
        if(currentTime.ToString(@"mm")=="00")
        {
            hourChanged?.Invoke(this, int.Parse(currentTime.ToString(@"hh")));
        }
        
        if (PercentOfDay() == 0) 
        {
            numDays++;
            dayChanged?.Invoke(this, numDays);
        }
        yield return new WaitForSeconds(minuteLenght);
        StartCoroutine(AddMinute());
    }

    private float PercentOfDay() { 
        return (float)currentTime.TotalMinutes % MinutesInDay / MinutesInDay;
    }
}
