using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string itemId;
    public Sprite itemSprite;
    
    [TextArea]
    public string pickupDialogue;

    private bool isPlayerInRange;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            if (InventoryManager.Instance != null && InventoryManager.Instance.CanInteract())
            {
                Pickup();
            }
        }
    }

    private void Pickup()
    {
        if (InventoryManager.Instance.IsFull())
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue("My pockets are full...");
            }
            return;
        }

        if (InventoryManager.Instance.AddItem(itemId, itemSprite))
        {
            if (!string.IsNullOrEmpty(pickupDialogue) && DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue(pickupDialogue);
            }

            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
        }
    }
}
