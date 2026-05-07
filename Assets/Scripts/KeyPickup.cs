using UnityEngine;

public class KeyPickup : MonoBehaviour
{
    public string keyId = "Room1Key";
    public Sprite keySprite;
    public string pickupDialogue = "I found a key";

    private bool isPlayerInRange;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            Pickup();
        }
    }

    private void Pickup()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(keyId, keySprite);
        }

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowDialogue(pickupDialogue);
        }

        // Disable this pickup so it only happens once
        gameObject.SetActive(false);
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
