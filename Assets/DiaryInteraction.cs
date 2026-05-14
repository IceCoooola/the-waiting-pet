using UnityEngine;

public class DiaryInteraction : MonoBehaviour
{
    [Header("Before getting lantern")]
    [TextArea]
    public string[] darkDialogueTexts;

    [Header("After getting lantern")]
    [TextArea]
    public string[] readableDialogueTexts;

    private bool isPlayerInRange;
    private int currentDialogueIndex = 0;
    private bool isDialogueShowing = false;
    private string[] currentDialogueTexts;
    private bool currentDialogueWasBeforeLantern = false;

    private void Update()
    {
        if (!isPlayerInRange) return;
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        if (DialogueManager.Instance == null) return;

        bool hasLantern = InventoryManager.Instance != null &&
                          InventoryManager.Instance.HasItem("Lantern");

        if (!isDialogueShowing)
        {
            currentDialogueWasBeforeLantern = !hasLantern;
            currentDialogueTexts = hasLantern ? readableDialogueTexts : darkDialogueTexts;

            if (currentDialogueTexts == null || currentDialogueTexts.Length == 0) return;

            currentDialogueIndex = 0;
            DialogueManager.Instance.ShowDialogue(currentDialogueTexts[currentDialogueIndex]);
            isDialogueShowing = true;
        }
        else
        {
            currentDialogueIndex++;

            if (currentDialogueIndex < currentDialogueTexts.Length)
            {
                DialogueManager.Instance.ShowDialogue(currentDialogueTexts[currentDialogueIndex]);
            }
            else
            {
                DialogueManager.Instance.HideDialogue();

                if (currentDialogueWasBeforeLantern && GameProgress.Instance != null)
                {
                    GameProgress.Instance.diaryRead = true;
                    Debug.Log("Diary has been read. Lantern is now available.");
                }

                isDialogueShowing = false;
                currentDialogueIndex = 0;
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