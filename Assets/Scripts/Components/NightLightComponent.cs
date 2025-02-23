using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightLightComponent : LightSourceComponent
{
    GameManager gameManager;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        gameManager = GameManager.Instance;
        gameManager.nightStart += OnNightStart;
        gameManager.nightEnd += OnNightEnd;
    }

    private void OnDestroy()
    {
        gameManager.nightStart -= OnNightStart;
        gameManager.nightEnd -= OnNightEnd;
    }

    private void OnNightStart(object sender, int hour)
    {
        SetLightEnabled(true);
    }

    private void OnNightEnd(object sender, int hour)
    {
        SetLightEnabled(false);
    }
}
