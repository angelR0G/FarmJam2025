using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.Port;

public class UIEffects : MonoBehaviour
{
    private GameObject fogObject;
    private GameObject fadeObject;
    private GameObject thunderObject;
    bool isDay = true;
    public static UIEffects Instance;

    Sequence fogSequence;
    Sequence uiEffectSequence;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void SetVisibleAll(bool active)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(active);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        GameManager gameManager = GameManager.Instance;
        gameManager.nightStart += IsNight;
        gameManager.dayStart += IsDay;
        fogObject = transform.GetChild(0).transform.gameObject;
        fadeObject = transform.GetChild(1).transform.gameObject;
        thunderObject = transform.GetChild(2).transform.gameObject;

        fogObject.GetComponent<Image>().material.SetFloat("_Opacity", 0f);

        SetVisibleAll(false);

        fadeObject.SetActive(true);

        Invoke("PerformFadeOut", 1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void IsNight(object sender, int v)
    {
        isDay = false;
    }

    public void IsDay(object sender, int v)
    {
        isDay = true;
    }

    public void StartFog(bool start)
    {
        SetVisibleAll(false);
        fogObject.SetActive(true);

        float startValue = start ? 0.0f : 1.0f;
        float endValue = start ? 1.0f : 0.0f;
        Material fog = fogObject.GetComponent<Image>().material;

        fog.SetFloat("_IsDay", isDay ? 0f : 1f);

        if (fogSequence != null && fogSequence.IsActive())
            fogSequence.Kill();

        fogSequence = DOTween.Sequence();
        fogSequence.Append(fog.DOFloat(endValue, "_Opacity", 2.0f))
            .OnComplete(()=> fogObject.SetActive(start))
            .OnKill(() => fogSequence = null);

        GameManager gameManager = GameManager.Instance;
        if (start)
        {
            gameManager.nightStart += DayFogToNightFog;
            gameManager.nightEnd += NightFogToDayFog;
        }
        else
        {
            gameManager.nightStart -= DayFogToNightFog;
            gameManager.nightEnd -= NightFogToDayFog;
        }
    }

    public void DayFogToNightFog(object sender, int hour)
    {
        Material fog = fogObject.GetComponent<Image>().material;
        float duration = GameManager.Instance.GetDuskDuration();

        fog.DOFloat(1, "_IsDay", duration).SetRecyclable(true);
    }

    public void NightFogToDayFog(object sender, int hour)
    {
        Material fog = fogObject.GetComponent<Image>().material;
        float duration = GameManager.Instance.GetDawnDuration();

        fog.DOFloat(0, "_IsDay", duration).SetRecyclable(true);
    }

    public void PerformThunder()
    {
        SetVisibleAll(false);
        if (uiEffectSequence != null && uiEffectSequence.IsActive())
            uiEffectSequence.Kill();
        Image thunderImage = thunderObject.GetComponent<Image>();
        uiEffectSequence = DOTween.Sequence();
        uiEffectSequence.AppendCallback(() => thunderObject.SetActive(true))
            .Append(thunderImage.DOFade(0.0f, 0.0f))
            .Append(thunderImage.DOFade(0.0f, 0.2f))
            .Append(thunderImage.DOFade(1.0f, 0.2f))
            .Append(thunderImage.DOFade(0.0f, 0.6f))
            .OnKill(() => uiEffectSequence = null);
    }

    public void PerformFadeIn()
    {
        SetVisibleAll(false);
        fadeObject.SetActive(true);

        if (uiEffectSequence != null && uiEffectSequence.IsActive())
            uiEffectSequence.Kill();
        float endValue = 1.0f;
        Image fadeImage = fadeObject.GetComponent<Image>();
        uiEffectSequence = DOTween.Sequence();
        uiEffectSequence.AppendCallback(() => fadeObject.SetActive(true))
            .Append(fadeImage.DOFade(endValue, 1.5f))
            //.OnComplete(()=> fadeObject.SetActive(false))
            .OnKill(() => uiEffectSequence = null);
        
    }
    public void PerformFadeOut()
    {
        SetVisibleAll(false);
        fadeObject.SetActive(true);

        if (uiEffectSequence != null && uiEffectSequence.IsActive())
            uiEffectSequence.Kill();

        float endValue = 0.0f;
        Image fadeImage = fadeObject.GetComponent<Image>();
        uiEffectSequence = DOTween.Sequence();
        uiEffectSequence.AppendCallback(() => fadeObject.SetActive(true))
            .Append(fadeImage.DOFade(endValue, 1.5f))
            .OnComplete(() => fadeObject.SetActive(false))
            .OnKill(() => uiEffectSequence = null);

    }
}
