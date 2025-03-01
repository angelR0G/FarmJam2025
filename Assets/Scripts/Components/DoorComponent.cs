using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorComponent : MonoBehaviour
{
    [Header("Components")]
    public SpriteRenderer spriteComponent;
    public AudioSource audioSource;

    [Header("Sprites")]
    public Sprite openedSprite;
    public Sprite closedSprite;

    [SerializeField, Header("Space point references")]
    private GameObject insidePoint;
    [SerializeField]
    private GameObject outsidePoint;
    [SerializeField]
    private bool fadeTopLayers = true;

    [Header("Sounds")]
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;

    bool isDoorOpened = false;

    private void Start()
    {
        spriteComponent = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        UpdateSprite();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<PlayerComponent>() != null)
        {
            isDoorOpened = true;

            audioSource.PlayOneShot(doorOpenSound);

            UpdateSprite();
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<PlayerComponent>() != null)
        {
            isDoorOpened = false;

            audioSource.PlayOneShot(doorCloseSound);

            UpdateSprite();

            if (fadeTopLayers)
                UpdateMapVisibility(collider.transform.position);
        }
    }

    private void UpdateSprite()
    {
        spriteComponent.sprite = isDoorOpened ? openedSprite : closedSprite;
    }

    private void UpdateMapVisibility(Vector3 playerPos)
    {
        float distanceToInside = (insidePoint.transform.position - playerPos).sqrMagnitude;
        float distanceToOutside = (outsidePoint.transform.position - playerPos).sqrMagnitude;

        MapComponent.Instance.SetTopTilemapsVisibility(distanceToOutside <= distanceToInside);
    }
}
