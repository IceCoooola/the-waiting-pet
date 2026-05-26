using UnityEngine;

public class CandlestickItem : MonoBehaviour
{
    [Header("Item")]
    public string itemId = "Candlestick";
    public Sprite itemIcon;

    [TextArea]
    public string pickupDialogue = "Picked up a candlestick.";

    [Header("Sound")]
    public AudioSource pickupAudio;

    private bool isPlayerInRange;
    private bool hasBeenPickedUp = false;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    private void Update()
    {
        if (hasBeenPickedUp) return;

        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            if (InventoryManager.Instance != null &&
                InventoryManager.Instance.CanInteract())
            {
                Pickup();
            }
        }
    }

    private void Pickup()
    {
        if (InventoryManager.Instance == null) return;

        if (InventoryManager.Instance.IsFull())
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue(
                    "I can't carry it, that's too many"
                );
            }

            return;
        }

        bool added = InventoryManager.Instance.AddItem(itemId, itemIcon);

        if (added)
        {
            hasBeenPickedUp = true;

            // Play pickup sound
            if (pickupAudio != null)
            {
                pickupAudio.Play();
            }

            // Show pickup dialogue
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue(pickupDialogue);
            }

            // Hide sprite
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = false;
            }

            // Disable collision
            if (col != null)
            {
                col.enabled = false;
            }

            // Destroy object after sound finishes
            Destroy(gameObject, 1.0f);
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