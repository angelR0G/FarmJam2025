using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaylightDestroyComponent : MonoBehaviour
{
    SpriteRenderer sprite;
    CarriableComponent carriable;
    InteractionTriggerComponent interactionTrigger;
    BloodContainer bloodContainer;

    private int safeAreasCount = 0;
    public int SafeAreasCount
    {
        get { return safeAreasCount; }
        set
        {
            int previousValue = safeAreasCount;
            safeAreasCount = value;

            if (previousValue >= 1 && value <= 0)
                DestroyIfDayTime();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        carriable = GetComponent<CarriableComponent>();
        interactionTrigger = GetComponent<InteractionTriggerComponent>();
        bloodContainer = GetComponent<BloodContainer>();

        GameManager.Instance.nightEnd += DestroyIfDayTimeCallback;

        // If the object is created in the day time, it will be destroyed
        Invoke("DestroyIfDayTime", 1f);
    }

    private void DestroyIfDayTimeCallback(object sender = null, int hour = 0)
    {
        DestroyIfDayTime();
    }

    private void DestroyIfDayTime()
    {
        if (safeAreasCount > 0 || GameManager.Instance.IsNightTime()) return;

        // If being carried, stop interaction
        if (carriable != null && carriable.carrier != null)
            carriable.carrier.ChangeState(carriable.carrier.walkingState);

        // Deactivates interaction
        if (interactionTrigger != null)
            Destroy(interactionTrigger);
        if (bloodContainer != null)
            Destroy(bloodContainer);

        // Destroys object
        if (sprite != null)
            sprite.DOFade(0, 1f).OnComplete(() => Destroy(gameObject)).SetRecyclable(true);
        else
            Destroy(gameObject);
    }

    private void OnDestroy()
    {
        GameManager.Instance.nightEnd -= DestroyIfDayTimeCallback;
    }
}
