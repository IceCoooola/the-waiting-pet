using UnityEngine;

public class DiaryInteraction : MonoBehaviour
{
    [Header("Before getting lantern")]
    [TextArea]
    public string[] darkDialogueTexts;

    [Header("After getting lantern")]
    [TextArea]
    public string[] readableDialogueTexts;

    [Header("Objects to reveal after fully reading diary")]
    public GameObject carpet;
    public GameObject[] footprints;

    private bool isPlayerInRange;
    private int currentDialogueIndex = 0;
    private bool isDialogueShowing = false;
    private string[] currentDialogueTexts;
    private bool currentDialogueWasBeforeLantern = false;

    private bool lanternUsed = false;

    private void Start()
    {
        if (carpet != null)
        {
            carpet.SetActive(false);
        }

        foreach (GameObject footprint in footprints)
        {
            if (footprint != null)
            {
                footprint.SetActive(false);
            }
        }
    }

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

            if (hasLantern && !lanternUsed)
            {
                InventoryManager.Instance.RemoveItem("Lantern");
                lanternUsed = true;
            }

            currentDialogueIndex = 0;
            DialogueManager.Instance.ShowDialogue(currentDialogueTexts[currentDialogueIndex], false);
            isDialogueShowing = true;
        }
        else
        {
            currentDialogueIndex++;

            if (currentDialogueIndex < currentDialogueTexts.Length)
            {
                DialogueManager.Instance.ShowDialogue(currentDialogueTexts[currentDialogueIndex], false);
            }
            else
            {
                DialogueManager.Instance.HideDialogue();

                if (currentDialogueWasBeforeLantern)
                {
                    if (GameProgress.Instance != null)
                    {
                        GameProgress.Instance.diaryRead = true;
                    }

                    Debug.Log("Diary has been read before getting lantern.");
                }
                else
                {
                    if (GameProgress.Instance != null)
                    {
                        GameProgress.Instance.diaryFullyRead = true;
                    }

                    RevealCarpetAndFootprints();
                    Debug.Log("Diary has been fully read. Carpet and footprints revealed.");
                }

                isDialogueShowing = false;
                currentDialogueIndex = 0;
            }
        }
    }

    private void RevealCarpetAndFootprints()
    {
        if (carpet != null)
        {
            carpet.SetActive(true);
        }

        foreach (GameObject footprint in footprints)
        {
            if (footprint != null)
            {
                footprint.SetActive(true);
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