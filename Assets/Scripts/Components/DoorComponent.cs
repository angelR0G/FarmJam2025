using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DoorComponent : MonoBehaviour
{
    [Header("Components")]
    public SpriteRenderer spriteComponent;
    public AudioSource audioSource;
    public Collider2D doorCollider;

    [Header("Events")]
    public UnityEvent onEnterPlace;
    public UnityEvent onExitPlace;

    [Header("Sprites")]
    public Sprite openedSprite;
    public Sprite closedSprite;

    [SerializeField, Header("Space point references")]
    private GameObject insidePoint;
    [SerializeField]
    private GameObject outsidePoint;
    [SerializeField]
    private bool isBuildingDoor = true;

    [Header("Sounds")]
    public AudioClip doorOpenSound;
    public AudioClip doorCloseSound;

    [SerializeField]
    bool isDoorBlocked = false;
    bool isDoorOpened = false;

    private void Start()
    {
        spriteComponent = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        SetDoorBlocked(isDoorBlocked);

        UpdateSprite();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (isDoorBlocked) return;

        if (collider.gameObject.TryGetComponent<PlayerComponent>(out PlayerComponent player))
        {
            isDoorOpened = true;

            if (doorOpenSound)
                audioSource.PlayOneShot(doorOpenSound);

            UpdateSprite();

            if (isBuildingDoor)
            {
                player.SetStepsSound(MapComponent.Instance.GetStepsSound(true));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (isDoorBlocked) return;

        if (collider.gameObject.TryGetComponent<PlayerComponent>(out PlayerComponent player))
        {
            isDoorOpened = false;

            if (doorCloseSound)
                audioSource.PlayOneShot(doorCloseSound);

            UpdateSprite();

            bool isPlayerOutside = IsOutside(collider.transform.position);
            MapComponent map = MapComponent.Instance;

            if (isBuildingDoor) {
                map.SetTopTilemapsVisibility(isPlayerOutside);
            }

            if (isPlayerOutside)
            {
                onExitPlace.Invoke();
                player.SetStepsSound(map.GetStepsSound(false));
            }
            else
                onEnterPlace.Invoke();
        }
    }

    private void UpdateSprite()
    {
        spriteComponent.sprite = isDoorOpened ? openedSprite : closedSprite;
    }

    private bool IsOutside(Vector3 playerPos)
    {
        float distanceToInside = (insidePoint.transform.position - playerPos).sqrMagnitude;
        float distanceToOutside = (outsidePoint.transform.position - playerPos).sqrMagnitude;

        return distanceToOutside <= distanceToInside;
    }

    public void SetDoorBlocked(bool newBlockedState)
    {
        isDoorBlocked = newBlockedState;

        doorCollider.excludeLayers = isDoorBlocked ? new LayerMask() : LayerMask.GetMask("Player");
    }
}
