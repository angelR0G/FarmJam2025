using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDyingState : PlayerState
{
    public override void EnterState()
    {
        player.animator.SetTrigger("Die");
        player.IsInteractionEnabled = false;

        UIEffects.Instance.Invoke("PerformFadeIn", 3f);
        Invoke("Respawn", 6f);
    }

    public override void ExitState()
    {
        UIEffects.Instance.Invoke("PerformFadeOut", 1f);
    }

    private void Respawn()
    {
        player.transform.position = player.spawnPosition;

        LoseItemsFromInventory();
        GameManager.Instance.currentMoney = Mathf.CeilToInt(GameManager.Instance.currentMoney / 2f);
        MapComponent.Instance.SetTopTilemapsVisibility(false);

        player.healthComponent.RestoreFullHealth();
        player.sanityComponent.RestoreFullSanity();

        // Manually change state to avoid restrictions
        player.currentState = null;
        player.ChangeState(player.walkingState);
        ExitState();
    }

    private void LoseItemsFromInventory()
    {
        for (int i = InventoryComponent.TOOLS_SLOTS; i< player.inventory.storageSize; i++)
        {
            ItemSlot itemSlot = player.inventory.GetItemByIndex(i);

            if (itemSlot.item == null || itemSlot.item.Type == ItemType.EvilCrop) continue;

            if (itemSlot.item.Type == ItemType.Tool) {
                player.inventory.RemoveItemByIndex(i, itemSlot.amount);
            }
            else if (itemSlot.item.Type == ItemType.Seed || itemSlot.item.Type == ItemType.Crop)
            {
                player.inventory.RemoveItemByIndex(i, Mathf.FloorToInt(itemSlot.amount / 2f));
            }
        }
    }
}
