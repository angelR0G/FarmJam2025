using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.Port;

public class UIEffects : MonoBehaviour
{
    private GameObject fogObject;
    private GameObject fadeObject;
    public static UIEffects Instance;
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
    // Start is called before the first frame update
    void Start()
    {
        fogObject = transform.GetChild(0).transform.gameObject;
        fadeObject = transform.GetChild(1).transform.gameObject;
        fogObject.SetActive(false);
        fadeObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartFog(bool start)
    {
        float startValue = start ? 0.0f : 1.0f;
        float endValue = start ? 1.0f : 0.0f;
        Material fog = fogObject.GetComponent<Image>().material;
        fog.SetFloat("_Opacity", startValue);
        uiEffectSequence = DOTween.Sequence();
        uiEffectSequence.AppendCallback(() => fogObject.SetActive(true))
            .Append(fog.DOFloat(endValue, "_Opacity", 2.0f))
            .OnComplete(()=> fogObject.SetActive(start))
            .OnKill(() => uiEffectSequence = null);
        fadeObject.SetActive(false);
    }

    public void PerformFadeIn(float time)
    {
        fogObject.SetActive(false);
        fadeObject.SetActive(true);
        if (uiEffectSequence != null && uiEffectSequence.IsActive())
            uiEffectSequence.Kill();
        float endValue = 1.0f;
        Image fadeImage = fadeObject.GetComponent<Image>();
        uiEffectSequence = DOTween.Sequence();
        uiEffectSequence.AppendCallback(() => fadeObject.SetActive(true))
            .Append(fadeImage.DOFade(endValue, 0.8f))
            .OnComplete(()=> fadeObject.SetActive(false))
            .OnKill(() => uiEffectSequence = null);
        
    }
    public void PerformFadeOut(float time)
    {
        fogObject.SetActive(false);
        fadeObject.SetActive(true);
        if (uiEffectSequence != null && uiEffectSequence.IsActive())
            uiEffectSequence.Kill();
        float endValue = 0.0f;
        Image fadeImage = fadeObject.GetComponent<Image>();
        uiEffectSequence = DOTween.Sequence();
        uiEffectSequence.AppendCallback(() => fadeObject.SetActive(true))
            .Append(fadeImage.DOFade(endValue, 0.8f))
            .OnComplete(() => fadeObject.SetActive(false))
            .OnKill(() => uiEffectSequence = null);

    }
}
