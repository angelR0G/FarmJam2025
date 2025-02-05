using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{

    [SerializeField]
    private GameManager gameManager;
    private TMP_Text text;
    [SerializeField]
    private TMP_Text textDay;


    private void Awake()
    {
        gameManager.worldTimeChanged += OnWorldTimeChanged;
        gameManager.dayChanged += OnDayChanged;
        gameManager.hourChanged += OnHourChanged;
        text = GetComponent<TMP_Text>();
    }
    private void OnWorldTimeChanged(object sender, TimeSpan newTime)
    {
        text.SetText(newTime.ToString(@"hh\:mm"));
    }
    private void OnDayChanged(object sender, int newDay)
    {
        textDay.SetText(newDay.ToString());
    }
    private void OnHourChanged(object sender, double newHour)
    {
        //Debug.Log("Hora + " + newHour);
    }
}
