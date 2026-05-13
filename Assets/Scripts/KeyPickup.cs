using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public string keyId = "Room1Key";
    public Sprite keySprite;
    public string pickupDialogue = "I found a key";
    public string emptyDialogue = "The bedside table is empty.";
    public bool deactivateOnPickup = true;

    private bool isPlayerInRange;
    private bool isPickedUp = false;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            if (InventoryManager.Instance != null && InventoryManager.Instance.CanInteract())
            {
                if (!isPickedUp)
                {
                    Pickup();
                }
                else
                {
                    ShowEmptyDialogue();
                }
            }
        }
    }

    private void ShowEmptyDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowDialogue(emptyDialogue);
        }
    }

    private void Pickup()
    {
        if (InventoryManager.Instance != null)
        {
            if (InventoryManager.Instance.IsFull())
            {
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.ShowDialogue("My pockets are full...");
                }
                return;
            }

            if (InventoryManager.Instance.AddItem(keyId, keySprite))
            {
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.ShowDialogue(pickupDialogue);
                }

                isPickedUp = true;
                if (deactivateOnPickup)
                {
                    gameObject.SetActive(false);
                }
            }
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
