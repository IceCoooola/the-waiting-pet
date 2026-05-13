using UnityEngine;

public class MultiPageDialogueInteraction : MonoBehaviour
{
    public string[] dialoguePages;
    private int currentPageIndex = -1;
    private bool isPlayerInRange;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            if (InventoryManager.Instance != null && !InventoryManager.Instance.CanInteract()) return;

            if (DialogueManager.Instance != null)
{
                currentPageIndex++;
                if (currentPageIndex < dialoguePages.Length)
                {
                    // Show current page. We use autoHide = false so it stays until next Space.
                    DialogueManager.Instance.ShowDialogue(dialoguePages[currentPageIndex], false);
                }
                else
                {
                    // End of dialogue
                    ResetDialogue();
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
            ResetDialogue();
        }
    }

    private void ResetDialogue()
    {
        currentPageIndex = -1;
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.HideDialogue();
        }
    }
}
