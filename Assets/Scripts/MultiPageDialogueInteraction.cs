using UnityEngine;

public class MultiPageDialogueInteraction : MonoBehaviour
{
    [Header("Dialogue")]
    public string[] dialoguePages;

    [Header("Sound")]
    public AudioSource pageFlipAudio;

    private int currentPageIndex = -1;
    private bool isPlayerInRange;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            if (InventoryManager.Instance != null &&
                !InventoryManager.Instance.CanInteract())
            {
                return;
            }

            if (DialogueManager.Instance != null)
            {
                currentPageIndex++;

                if (currentPageIndex < dialoguePages.Length)
                {
                    // Play page flip sound when moving to a dialogue page
                    if (pageFlipAudio != null)
                    {
                        pageFlipAudio.Play();
                    }

                    // Show current page. autoHide = false keeps it visible until next Space.
                    DialogueManager.Instance.ShowDialogue(
                        dialoguePages[currentPageIndex],
                        false,
                        0, null, false, true
                    );
                }
                else
                {
                    // End dialogue
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