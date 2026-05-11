using UnityEngine;

public class CandlestickItem : MonoBehaviour
{
    public string itemId = "Candlestick";
    public Sprite itemIcon;
    public string pickupDialogue = "Picked up a candlestick.";

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
        if (InventoryManager.Instance != null)
        {
            if (InventoryManager.Instance.IsFull())
            {
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.ShowDialogue("I can't carry it, that's too many");
                }
                return;
            }

            if (InventoryManager.Instance.AddItem(itemId, itemIcon))
            {
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.ShowDialogue(pickupDialogue);
                }
                gameObject.SetActive(false);
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
