using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenComponent : MonoBehaviour
{
    private void Start()
    {
        GetComponent<HealthComponent>().onDieCallback = OnChickenKilled;
    }
    public void OnChickenKilled()
    {
        GameObject childObject = transform.GetChild(0).gameObject;

        childObject.GetComponent<Animator>().StopPlayback();
        SpriteRenderer sprite = childObject.GetComponent<SpriteRenderer>();

        Sequence sequence = DOTween.Sequence().SetRecyclable(true);

        sequence.Append(sprite.DOColor(new Color(1, 0, 0), 3f).SetRecyclable(true))
            .Append(childObject.transform.DOScale(5, 8f).SetRecyclable(true))
            .Append(childObject.transform.DOScale(0, 0.5f).SetRecyclable(true))
            .Join(sprite.DOFade(0, 0.5f).SetRecyclable(true))
            .OnComplete(() => Destroy(gameObject));
    }
}
