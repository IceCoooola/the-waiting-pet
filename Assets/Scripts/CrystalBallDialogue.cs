using UnityEngine;

public class CrystalBallDialogue : MonoBehaviour
{
    public string itemId;
    public bool isStoryCompleted { get; private set; }
    
    public CrystalBallDialogue[] dependencies;
    
    [TextArea]
    public string prePickupDialogue = "I saw something in the crystal ball.... I can't see it clearly.... seems like a ...";

    [TextArea]
    public string lockedDialogue = "The mist inside won't clear... I feel like I'm missing something important.";
    
    [TextArea]
    public string[] postPickupDialogues;

    private const string SPACE_PROMPT = "\n(Press space to continue.)";
    private int currentPageIndex = -1;
    private bool isPlayerInRange;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            if (InventoryManager.Instance != null && !InventoryManager.Instance.CanInteract()) return;

            if (InventoryManager.Instance != null && InventoryManager.Instance.HasItem(itemId))
            {
                if (AreDependenciesMet())
                {
                    ShowPostPickupDialogue();
                }
                else
                {
                    ShowLockedDialogue();
                }
            }
            else
            {
                ShowPrePickupDialogue();
            }
        }
    }

    private bool AreDependenciesMet()
    {
        if (dependencies == null || dependencies.Length == 0) return true;
        foreach (var dep in dependencies)
        {
            if (dep != null && !dep.isStoryCompleted) return false;
        }
        return true;
    }

    private void ShowLockedDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowDialogue(lockedDialogue + SPACE_PROMPT, false);
        }
    }

    private void ShowPrePickupDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            // We set autoHide to false so the player actually has to press space as prompted
            DialogueManager.Instance.ShowDialogue(prePickupDialogue + SPACE_PROMPT, false);
        }
    }

    private void ShowPostPickupDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            currentPageIndex++;
            if (currentPageIndex < postPickupDialogues.Length)
            {
                DialogueManager.Instance.ShowDialogue(postPickupDialogues[currentPageIndex] + SPACE_PROMPT, false);
                
                // Mark as completed when the last page is reached
                if (currentPageIndex == postPickupDialogues.Length - 1)
                {
                    isStoryCompleted = true;
                }
            }
            else
            {
                ResetDialogue();
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
