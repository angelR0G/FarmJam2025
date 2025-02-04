using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    private int health = 0;
    [SerializeField]
    private int maxHealth = 100;
    public Action onDieCallback;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    public void LooseHealth(int h)
    {
        health -= h;
        
        if (health <= 0) onDieCallback();
    }

    public void RestoreHealth(int h)
    {
        health = Math.Min(health + h, maxHealth);
    }
}
