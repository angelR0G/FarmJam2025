using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeDisplay : MonoBehaviour
{

    [SerializeField]
    private GameManager gameManager;
    private TMP_Text text;
    [SerializeField]
    private TMP_Text textDay;
    [SerializeField]
    private TMP_Text textMoney;
    [SerializeField]
    private Image timeCircleImage;


    private void Awake()
    {
        gameManager.worldTimeChanged += OnWorldTimeChanged;
        gameManager.dayChanged += OnDayChanged;
        gameManager.hourChanged += OnHourChanged;
        gameManager.moneyChanged += OnMoneyChanged;
        text = GetComponent<TMP_Text>();
    }

    private void OnDestroy()
    {
        gameManager.worldTimeChanged -= OnWorldTimeChanged;
        gameManager.dayChanged -= OnDayChanged;
        gameManager.hourChanged -= OnHourChanged;
    }
    private void OnWorldTimeChanged(object sender, TimeSpan newTime)
    {
        text.SetText(newTime.ToString(@"hh\:mm"));
        float angle = (gameManager.PercentOfDay() *0.995f) * 360f;
        timeCircleImage.rectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
    private void OnDayChanged(object sender, int newDay)
    {
        textDay.SetText(newDay.ToString());
    }
    private void OnHourChanged(object sender, double newHour)
    {
        //Debug.Log("Hora + " + newHour);
    }
    private void OnMoneyChanged(object sender, int newHour)
    {
        textMoney.SetText(newHour.ToString());
    }
}
