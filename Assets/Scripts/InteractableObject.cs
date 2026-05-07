using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public string dialogueText = "a clock.";
    private bool isPlayerInRange;
    private DialogueManager dialogueManager;

    private void Start()
    {
        dialogueManager = Object.FindFirstObjectByType<DialogueManager>();
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            if (dialogueManager != null)
            {
                dialogueManager.ShowDialogue(dialogueText);
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
            if (dialogueManager != null)
            {
                dialogueManager.HideDialogue();
            }
        }
    }
}
