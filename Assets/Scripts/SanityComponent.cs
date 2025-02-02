using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SanityComponent : MonoBehaviour
{
    private int sanity = 0;
    [SerializeField]
    private int maxSanity = 100;

    // Start is called before the first frame update
    void Start()
    {
        sanity = maxSanity;
    }

    public void LooseSanity(int s)
    {
        sanity -= s;
    }

    public void RestoreSanity(int s)
    {
        sanity = Math.Min(sanity + s, maxSanity);
    }
}
