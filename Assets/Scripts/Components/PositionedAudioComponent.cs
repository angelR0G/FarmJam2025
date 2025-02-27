using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PositionedAudioComponent : MonoBehaviour
{
    public AudioSource audioSource;
    public float minDistance;
    public float maxDistance;
    private static Transform listenerTransform;
    [SerializeField]
    private float volumeMultiplier = 1f;

    private float sqrMinDistance = 0;
    private float sqrMaxDistance = 0;
    private float updateTimer = 0;

    public float VolumeMultiplier {  get { return volumeMultiplier; } set { volumeMultiplier = Mathf.Max(value, 0); } }

    // Start is called before the first frame update
    void Start()
    {
        if (listenerTransform == null)
            listenerTransform = Camera.main.transform.parent;

        audioSource = GetComponent<AudioSource>();

        sqrMinDistance = Mathf.Pow(minDistance, 2);
        sqrMaxDistance = Mathf.Pow(maxDistance, 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (updateTimer > 0)
        {
            updateTimer -= Time.deltaTime;
            return;
        }

        Vector3 listenerVector = listenerTransform.position - transform.position;
        float sqrDistance = listenerVector.sqrMagnitude;

        if (sqrDistance >= sqrMaxDistance) {
            audioSource.volume = 0;

            // Reduce sound updates when listener is too far to hear it
            updateTimer = sqrDistance > sqrMaxDistance + 5 ? 2f : 0.2f;
        }
        else if (sqrDistance <= sqrMinDistance)
            audioSource.volume = volumeMultiplier;
        else
        {
            audioSource.volume = (1f - (sqrDistance - sqrMinDistance)/(sqrMaxDistance - sqrMinDistance)) * volumeMultiplier;
        }

        if (sqrDistance < sqrMaxDistance)
        {
            float panning = -listenerVector.x/maxDistance;
            audioSource.panStereo = Mathf.Clamp(panning, -0.8f, 0.8f);
        }
    }
}
