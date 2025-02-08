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
    public event EventHandler<int> moneyChanged;
    public event EventHandler<int> hourChanged;

    public const int MinutesInDay = 1440;

    [SerializeField]
    private float dayLength;

    private TimeSpan currentTime;
    private float minuteLenght => dayLength / MinutesInDay;
    [SerializeField]
    private Gradient gradient;

    private Light2D sunSource;

    [SerializeField]
    private int numDays;

    public int currentMoney = 0;

    [Header("Gradient Configure")]
    [SerializeField] private int nightDuration = 4;
    [SerializeField] private int nighStartDuration = 6;
    [SerializeField] private int dayDuration = 11;
    [SerializeField] private int dawnDuration = 1;
    [SerializeField] private int duskDuration = 2;

    [SerializeField] private Color NIGHT_COLOR = new Color(0.1553594f, 0.1443129f, 0.6509434f);
    [SerializeField] private Color DAY_COLOR = new Color(1f, 1f, 1f);
    [SerializeField] private Color DAWN_COLOR = new Color(1f, 0.7158657f, 0.1745283f);
    [SerializeField] private Color DUSK_DOWN = new Color(1f, 0.7158657f, 0.1745283f);



    private void Awake()
    {
        sunSource = GetComponent<Light2D>();
        gradient = new Gradient();
        GradientColorKey[] gradientColorKeys = { 
            new GradientColorKey(NIGHT_COLOR, 0),
            new GradientColorKey(DAWN_COLOR, ((nighStartDuration+dawnDuration/2)*60*minuteLenght)/(24*60*minuteLenght)),
            new GradientColorKey(DAY_COLOR, ((nighStartDuration+dawnDuration) * 60 * minuteLenght)/(24*60*minuteLenght)), 
            new GradientColorKey(DAY_COLOR, ((nighStartDuration+dawnDuration+dayDuration) * 60 * minuteLenght)/(24*60*minuteLenght)), 
            new GradientColorKey(DUSK_DOWN, ((nighStartDuration+dawnDuration+dayDuration+duskDuration/2) * 60 * minuteLenght)/(24*60*minuteLenght)), 
            new GradientColorKey(NIGHT_COLOR, ((nighStartDuration+dawnDuration+dayDuration+duskDuration) * 60 * minuteLenght)/(24*60*minuteLenght)),
            new GradientColorKey(NIGHT_COLOR, ((nighStartDuration+dawnDuration+dayDuration+duskDuration+nightDuration) * 60 * minuteLenght)/(24*60*minuteLenght)) 
        };
        GradientAlphaKey[] gradientAlphaKeys = { };
        gradient.SetKeys(gradientColorKeys, gradientAlphaKeys);
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
        sunSource.color = gradient.Evaluate(PercentOfDay());
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

    public float PercentOfDay() { 
        return (float)currentTime.TotalMinutes % MinutesInDay / MinutesInDay;
    }

    public void UpdateMoney(int quantity)
    {
        currentMoney += quantity;
        moneyChanged?.Invoke(this, currentMoney);
    }
}
