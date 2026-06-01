using UnityEngine;
using System.Collections;

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

    [Header("Page Turning Sound")]
    public AudioSource audioSource;
    public AudioClip pageTurnClip;

    [Header("Puzzle Hint")]
    public float hintDelay = 180f;

    [TextArea]
    public string puzzleHintDialogue =
        "I noticed something about the objects on the table.\nOne side is noticeably larger than the other.\nThe wider side always seems to lead.";

    [Header("Puzzle Reward Key")]
    public GameObject puzzleRewardKey;

    private bool isPlayerInRange;
    private int currentDialogueIndex = 0;
    private bool isDialogueShowing = false;
    private string[] currentDialogueTexts;
    private bool currentDialogueWasBeforeLantern = false;

    private bool diaryUnlocked = false;
    private bool puzzleHintShown = false;
    private Coroutine puzzleHintCoroutine;

    private void Start()
    {
        if (GameProgress.Instance != null && GameProgress.Instance.diaryFullyRead)
        {
            diaryUnlocked = true;
            RevealCarpetAndFootprints();
            StartPuzzleHintTimer();
        }
        else
        {
            if (carpet != null) carpet.SetActive(false);

            foreach (GameObject footprint in footprints)
            {
                if (footprint != null) footprint.SetActive(false);
            }
        }
    }

    private void Update()
    {
        if (!isPlayerInRange) return;
        if (!Input.GetKeyDown(KeyCode.Space)) return;
        if (DialogueManager.Instance == null) return;

        if (InventoryManager.Instance != null && !InventoryManager.Instance.CanInteract()) return;

        bool hasLantern = InventoryManager.Instance != null &&
                          InventoryManager.Instance.HasItem("Lantern");

        bool canReadDiary = diaryUnlocked || hasLantern;

        if (!isDialogueShowing)
        {
            currentDialogueWasBeforeLantern = !canReadDiary;
            currentDialogueTexts = canReadDiary ? readableDialogueTexts : darkDialogueTexts;

            if (currentDialogueTexts == null || currentDialogueTexts.Length == 0) return;

            if (hasLantern && !diaryUnlocked)
            {
                diaryUnlocked = true;
                InventoryManager.Instance.RemoveItem("Lantern");
            }

            currentDialogueIndex = 0;

            PlayPageTurnSoundIfReadable();

            DialogueManager.Instance.ShowDialogue(
                currentDialogueTexts[currentDialogueIndex],
                false, 0, null, false, true
            );

            isDialogueShowing = true;
        }
        else
        {
            currentDialogueIndex++;

            if (currentDialogueIndex < currentDialogueTexts.Length)
            {
                PlayPageTurnSoundIfReadable();

                DialogueManager.Instance.ShowDialogue(
                    currentDialogueTexts[currentDialogueIndex],
                    false, 0, null, false, true
                );
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
                }
                else
                {
                    if (GameProgress.Instance != null)
                    {
                        GameProgress.Instance.diaryFullyRead = true;
                    }

                    RevealCarpetAndFootprints();
                    StartPuzzleHintTimer();
                }

                isDialogueShowing = false;
                currentDialogueIndex = 0;
            }
        }
    }

    private void StartPuzzleHintTimer()
    {
        if (puzzleHintShown) return;
        if (puzzleHintCoroutine != null) return;

        puzzleHintCoroutine = StartCoroutine(ShowPuzzleHintAfterDelay());
    }

    private IEnumerator ShowPuzzleHintAfterDelay()
    {
        yield return new WaitForSeconds(hintDelay);

        if (puzzleHintShown) yield break;
        if (DialogueManager.Instance == null) yield break;

        if (puzzleRewardKey != null && puzzleRewardKey.activeInHierarchy)
        {
            yield break;
        }

        while (DialogueManager.Instance != null &&
               DialogueManager.Instance.IsDialogueActive())
        {
            yield return null;
        }

        if (puzzleRewardKey != null && puzzleRewardKey.activeInHierarchy)
        {
            yield break;
        }

        puzzleHintShown = true;

        DialogueManager.Instance.ShowDialogue(
            puzzleHintDialogue,
            false,
            0,
            null,
            false,
            false
        );
    }

    private void PlayPageTurnSoundIfReadable()
    {
        if (currentDialogueWasBeforeLantern) return;
        if (audioSource == null || pageTurnClip == null) return;

        audioSource.PlayOneShot(pageTurnClip);
    }

    private void RevealCarpetAndFootprints()
    {
        if (carpet != null) carpet.SetActive(true);

        foreach (GameObject footprint in footprints)
        {
            if (footprint != null) footprint.SetActive(true);
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