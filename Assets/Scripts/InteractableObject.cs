using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string dialogueText = "a clock.";
    private bool isPlayerInRange;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            if (InventoryManager.Instance != null && !InventoryManager.Instance.CanInteract()) return;

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue(dialogueText);
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
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.HideSingleDialogue();
            }
        }
    }
}
