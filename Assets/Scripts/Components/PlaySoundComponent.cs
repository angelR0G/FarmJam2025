using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundComponent : MonoBehaviour
{
    public AudioClip audioClip;
    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(audioClip);
        Destroy(this);
    }
}
