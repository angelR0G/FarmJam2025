using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HealthComponent : MonoBehaviour
{
    private const float DAMAGE_EFFECT_TIME = 0.2f;

    [SerializeField]
    private int health = 0;
    [SerializeField]
    private int maxHealth = 100;
    public Action onDieCallback;
    public UnityEvent onDamageEvent;
    private float damagedRemainingTime;
    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        sprite = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        ApplyDamagedEffect();
    }

    public void LooseHealth(int h)
    {
        if (health <= 0) return;

        health -= h;

        if (health <= 0) 
        {
            if (onDieCallback != null)
                onDieCallback();
        }
        else
        {
            damagedRemainingTime = DAMAGE_EFFECT_TIME;
            onDamageEvent.Invoke();
        }
    }

    public void RestoreHealth(int h)
    {
        health = Math.Min(health + h, maxHealth);
    }

    public void Kill()
    {
        health = 0;
        onDieCallback();
    }

    private void ApplyDamagedEffect()
    {
        if (damagedRemainingTime <= 0 || !sprite) return;

        damagedRemainingTime -= Time.deltaTime;
        sprite.color = damagedRemainingTime > 0 ? Color.red : Color.white;
    }

    public bool IsStunned()
    {
        return damagedRemainingTime > 0;
    }

    public int GetHealth() { return health; }
    public float GetHealthPercentage() { return Math.Clamp((float)health/ (float)maxHealth, 0, 1); }
}
