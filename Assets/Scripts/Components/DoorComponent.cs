using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorComponent : MonoBehaviour
{
    public Sprite openedSprite;
    public Sprite closedSprite;
    public SpriteRenderer spriteComponent;

    [SerializeField]
    private GameObject insidePoint;
    [SerializeField]
    private GameObject outsidePoint;

    bool isDoorOpened = false;

    private void Start()
    {
        spriteComponent = GetComponent<SpriteRenderer>();
        UpdateSprite();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<PlayerComponent>() != null)
        {
            isDoorOpened = true;

            UpdateSprite();
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<PlayerComponent>() != null)
        {
            isDoorOpened = false;

            UpdateSprite();
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
