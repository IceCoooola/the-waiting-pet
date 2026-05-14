using UnityEngine;

public class LanternPickup : MonoBehaviour
{
    public Sprite lanternIcon;
    public string itemId = "Lantern";

    [Header("Dialogue")]
    public string pickupDialogue = "I found a lantern.";
    public string lockedDialogue = "Maybe I should read the diary first.";

    private bool isPlayerInRange;
    private bool isPickedUp = false;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Space) && !isPickedUp)
        {
            if (InventoryManager.Instance == null) return;
            if (GameProgress.Instance == null) return;

            if (!GameProgress.Instance.diaryRead)
            {
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.ShowDialogue(lockedDialogue);
                }

                return;
            }

            bool added = InventoryManager.Instance.AddItem(itemId, lanternIcon);

            if (added)
            {
                isPickedUp = true;

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