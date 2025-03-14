using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Light2D))]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public event EventHandler<int> moneyChanged;
    public event EventHandler<int> dayChanged;
    public event EventHandler<int> hourChanged;
    public event EventHandler<int> nightStart;
    public event EventHandler<int> nightEnd;
    public event EventHandler<int> dayStart;

    // Total duration of day/hours (in seconds)
    [SerializeField, Range(0, 24)]
    private int initialHour;
    [SerializeField]
    private float dayLength;
    private float hourLength;

    // Timers that increase automatically and reset each day/hour
    private float currentDayTimer;
    private float currentHourTimer;

    // The numbers that represent current state
    private int numDays;
    private int numHours;

    [Header("Day key hours")]
    [SerializeField] private int nightStartHour = 22;
    [SerializeField] private int nightEndHour = 6;
    [SerializeField] private int dawnDuration = 1;
    [SerializeField] private int duskDuration = 1;
    [SerializeField] private bool startPaused = false;

    // Light color
    [SerializeField]
    private Gradient gradient;

    [SerializeField] private Color NIGHT_COLOR = new Color(0.1553594f, 0.1443129f, 0.6509434f);
    [SerializeField] private Color DAY_COLOR = new Color(1f, 1f, 1f);
    [SerializeField] private Color DAWN_COLOR = new Color(1f, 0.7158657f, 0.1745283f);
    [SerializeField] private Color DUSK_DOWN = new Color(1f, 0.7158657f, 0.1745283f);

    [Header("Sounds")]
    public AudioClip dayAmbience;
    public AudioClip nightAmbience;
    public AudioClip newDaySound;

    private bool isDayNightCyclePaused = false;
    public int currentMoney = 0;

    private Light2D sunSource;
    private AudioSource audioSource;

    [Header("Pause menu")]
    [SerializeField] private GameObject pauseMenu;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        CalculateDayNightCycleTimes();
        ResetTimeToFirstDay();
        InitGradient();

        sunSource = GetComponent<Light2D>();
        audioSource = GetComponent<AudioSource>();

        PauseDayNightCycle(startPaused);
    }

    private void Start()
    {
        if (newDaySound)
            audioSource.PlayOneShot(newDaySound);
    }

    private void Update()
    {
        UpdateTime();
        sunSource.color = gradient.Evaluate(PercentOfDay());
    }

    public float PercentOfDay() { 
        return currentDayTimer/dayLength;
    }

    public void UpdateTime()
    {
        if (isDayNightCyclePaused) return;

        currentDayTimer += Time.deltaTime;
        currentHourTimer += Time.deltaTime;

        if (currentHourTimer >= hourLength && numHours < 23)
        {
            numHours++;
            hourChanged?.Invoke(this, numHours);

            currentHourTimer -= hourLength;
            CallEventsByHour(numHours);
            UpdateAmbience();
        }
        else if (currentDayTimer >= dayLength)
        {
            numDays++;
            numHours = 0;

            dayChanged?.Invoke(this, numDays);
            hourChanged?.Invoke(this, 0);

            currentDayTimer -= dayLength;
            currentHourTimer = currentDayTimer;
        }
    }

    public void UpdateMoney(int quantity)
    {
        currentMoney += quantity;
        moneyChanged?.Invoke(this, currentMoney);
    }

    public void UpdateAmbience()
    {
        if (numHours == nightStartHour - duskDuration)
        {
            if (nightAmbience != null)
            {
                Sequence sequence = DOTween.Sequence().SetRecyclable(true);

                sequence.Append(audioSource.DOFade(0, 10f).SetRecyclable(true))
                    .AppendCallback(() => { audioSource.clip = nightAmbience; audioSource.Play(); })
                    .Append(audioSource.DOFade(1, 10f).SetRecyclable(true)).SetDelay(duskDuration*hourLength / 2);
            }
        }
        else if (numHours == nightEndHour)
        {
            if (dayAmbience != null)
            {
                Sequence sequence = DOTween.Sequence().SetRecyclable(true);

                sequence.Append(audioSource.DOFade(0, 10f).SetRecyclable(true))
                    .AppendCallback(() => { audioSource.clip = dayAmbience; audioSource.Play(); })
                    .Append(audioSource.DOFade(1, 10f).SetRecyclable(true)).SetDelay(dawnDuration*hourLength / 2);
            }
        }

        if (numHours == nightEndHour + dawnDuration)
        {
            if (newDaySound)
                audioSource.PlayOneShot(newDaySound);
        }
    }

    public void PauseDayNightCycle(bool newState)
    {
        isDayNightCyclePaused = newState;
    }

    private void CalculateDayNightCycleTimes()
    {
        hourLength = dayLength / 24f;
    }

    public void ResetTimeToFirstDay()
    {
        numDays = 0;
        numHours = initialHour;
        currentDayTimer = dayLength * (initialHour/24f);
        currentHourTimer = 0;
    }

    public void CallEventsByHour(int hour)
    {
        if (hour == nightStartHour)
            nightStart?.Invoke(this, hour);

        else if (hour == nightEndHour)
            nightEnd?.Invoke(this, hour);

        else if (hour == nightEndHour + dawnDuration)
            dayStart?.Invoke(this, hour);
    }

    public void InitGradient()
    {
        gradient = new Gradient();
        GradientColorKey[] gradientColorKeys = {
            new GradientColorKey(NIGHT_COLOR, 0),
            new GradientColorKey(NIGHT_COLOR, nightEndHour/24f),
            new GradientColorKey(DAWN_COLOR, ((float)nightEndHour + dawnDuration/2f)/24f),
            new GradientColorKey(DAY_COLOR, (nightEndHour + dawnDuration)/24f),
            new GradientColorKey(DAY_COLOR, (nightStartHour - duskDuration)/24f),
            new GradientColorKey(DUSK_DOWN, ((float)nightStartHour - duskDuration/2f)/24f),
            new GradientColorKey(NIGHT_COLOR, (nightStartHour)/24f),
            new GradientColorKey(NIGHT_COLOR, 1)
        };

        GradientAlphaKey[] gradientAlphaKeys = { };
        gradient.SetKeys(gradientColorKeys, gradientAlphaKeys);
    }

    public bool IsNightTime()
    {
        return numHours >= nightStartHour || numHours < nightEndHour;
    }

    public float GetDuskDuration()
    {
        return duskDuration * hourLength;
    }

    public float GetDawnDuration()
    {
        return dawnDuration * hourLength;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    
}
