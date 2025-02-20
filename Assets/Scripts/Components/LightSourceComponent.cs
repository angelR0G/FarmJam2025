using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class LightSourceComponent : MonoBehaviour
{

    private GameManager gameManager;
    private Light2D light2D;
    private CircleCollider2D collider;
    public UnityAction onEnterInLight;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        gameManager.nightStart += OnNightStart;
        gameManager.nightEnd += OnNightEnd;
        light2D = GetComponent<Light2D>();
        collider = GetComponent<CircleCollider2D>();
        light2D.enabled = true;
        collider.enabled = true;
    }

    private void OnDestroy()
    {
        gameManager.nightStart -= OnNightStart;
        gameManager.nightEnd -= OnNightEnd;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnNightStart(object sender, int hour)
    {
        light2D.enabled = true;
        collider.enabled=true;
    }

    private void OnNightEnd(object sender, int hour)
    {
        light2D.enabled = false;
        collider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        SanityComponent sanity;

        if (other.TryGetComponent<SanityComponent>(out sanity))
        {
            sanity.insideLightSource = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        SanityComponent sanity;

        if (other.TryGetComponent<SanityComponent>(out sanity))
        {
            sanity.insideLightSource = false;
        }
    }
}
