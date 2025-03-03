using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockEvilSeedAtDayComponent : MonoBehaviour
{
    GameManager gameManager;
    PlayerComponent player;
    public DoorComponent door;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerComponent p;
        if (collision.TryGetComponent<PlayerComponent>(out p))
        {
            player = p;

            if (IsCarringEvilSeedAtDay())
            {
                DialogueSystem.Instance.DisplayDialogue(new Dialogue("If I take the seed outside during the day, it will be consumed. I have to wait until the sun goes down.", 1, false, 6f));
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (player != null && collision.gameObject == player.gameObject)
            player = null;
    }

    private void Update()
    {
        if (player != null)
        {
            // Block the door if the player is carring an evil seed during night
            door.SetDoorBlocked(IsCarringEvilSeedAtDay());
        }
    }

    private bool IsCarringEvilSeedAtDay()
    {
        if (player == null || gameManager.IsNightTime()) return false;

        return player.currentState == player.carringState && player.carringState.carriedObject.GetComponent<CropComponent>();
    }
}
