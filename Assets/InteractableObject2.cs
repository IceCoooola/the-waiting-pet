using UnityEngine;

public class InteractableObject2 : MonoBehaviour
{
    [TextArea]
    public string[] dialogueTexts;

    private bool isPlayerInRange;
    private int currentDialogueIndex = 0;
    private bool isDialogueShowing = false;

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            if (DialogueManager.Instance == null) return;
            if (dialogueTexts == null || dialogueTexts.Length == 0) return;

            // Start dialogue
            if (!isDialogueShowing)
            {
                currentDialogueIndex = 0;
                DialogueManager.Instance.ShowDialogue(dialogueTexts[currentDialogueIndex]);
                isDialogueShowing = true;
            }
            else
            {
                // Move to next dialogue
                currentDialogueIndex++;

                if (currentDialogueIndex < dialogueTexts.Length)
                {
                    DialogueManager.Instance.ShowDialogue(dialogueTexts[currentDialogueIndex]);
                }
                else
                {
                    // End dialogue
                    DialogueManager.Instance.HideDialogue();
                    isDialogueShowing = false;
                    currentDialogueIndex = 0;
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

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.HideDialogue();
            }

            isDialogueShowing = false;
            currentDialogueIndex = 0;
        }
    }
}