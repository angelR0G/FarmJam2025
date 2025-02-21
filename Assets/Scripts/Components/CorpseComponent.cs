using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CorpseComponent : MonoBehaviour
{
    public CorpseCreature creature;
    public UnityEvent onCorpseConsumed;

    public void ConsumeCorpse()
    {
        onCorpseConsumed.Invoke();
        Destroy(gameObject);
    }
}

public enum CorpseCreature
{
    Pig,
    Stalker
}
