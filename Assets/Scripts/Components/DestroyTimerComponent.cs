using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimerComponent : MonoBehaviour
{
    public float timer = 1f;

    // Start is called before the first frame update
    void Start()
    {
        Invoke("DestroyObject", timer);
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
