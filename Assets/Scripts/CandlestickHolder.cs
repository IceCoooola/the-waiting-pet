using UnityEngine;

public class CandlestickHolder : MonoBehaviour
{
    [Header("Spots")]
    public GameObject leftSpot;
    public GameObject middleSpot;
    public GameObject rightSpot;

    [Header("Item Info")]
    public string candlestickId = "Candlestick";
    public Sprite candlestickIcon;

    [Header("Dialogue")]
    [TextArea]
    public string firstInstructionDialogue =
        "Press 1, 2, or 3 to place or take back a candlestick.\n" +
        "1 = Left, 2 = Middle, 3 = Right\n" +
        "Press Space to close.";

    [TextArea]
    public string shortInstructionDialogue =
        "1 = Left, 2 = Middle, 3 = Right\n" +
        "Press Space to close.";

    [TextArea]
    public string emptyDialogue = "Empty candlestick...";

    [TextArea]
    public string needCandlestickDialogue = "I need a candlestick.";

    [TextArea]
    public string fullInventoryDialogue = "I can't carry more.";

    private bool isPlayerInRange = false;
    private bool choosingSpot = false;

    private static bool hasShownInstructionOnce = false;

    private void Update()
    {
        if (!isPlayerInRange) return;

        // Space = open OR close interaction
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (choosingSpot)
            {
                EndSpotSelection();
            }
            else
            {
                StartSpotSelection();
            }

            return;
        }

        // While interaction is active
        if (choosingSpot)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                ToggleSpot(leftSpot);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ToggleSpot(middleSpot);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ToggleSpot(rightSpot);
            }
        }
    }

    private void StartSpotSelection()
    {
        bool hasHand =
            InventoryManager.Instance != null &&
            InventoryManager.Instance.HasItem(candlestickId);

        int candleCount = GetCandleCount();

        // Nothing to interact with
        if (!hasHand && candleCount == 0)
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue(emptyDialogue, false);
            }

            return;
        }

        choosingSpot = true;

        if (DialogueManager.Instance == null) return;

        if (!hasShownInstructionOnce)
        {
            hasShownInstructionOnce = true;
            DialogueManager.Instance.ShowDialogue(firstInstructionDialogue, false);
        }
        else
        {
            DialogueManager.Instance.ShowDialogue(shortInstructionDialogue, false);
        }
    }

    private void EndSpotSelection()
    {
        choosingSpot = false;

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.HideDialogue();
        }
    }

    private void ToggleSpot(GameObject spot)
    {
        if (spot == null) return;
        if (InventoryManager.Instance == null) return;

        // If candle exists there -> take back
        if (spot.activeSelf)
        {
            TakeBackFromSpot(spot);
        }
        // Otherwise -> place
        else
        {
            PlaceOnSpot(spot);
        }

        CandlestickPuzzleManager manager =
            Object.FindFirstObjectByType<CandlestickPuzzleManager>();

        if (manager != null)
        {
            manager.CheckPuzzle();
        }

        // Keep dialogue active after interaction
        if (DialogueManager.Instance != null && choosingSpot)
        {
            DialogueManager.Instance.ShowDialogue(shortInstructionDialogue, false);
        }
    }

    private void PlaceOnSpot(GameObject spot)
    {
        if (!InventoryManager.Instance.HasItem(candlestickId))
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue(needCandlestickDialogue, false);
            }

            return;
        }

        InventoryManager.Instance.RemoveItem(candlestickId);

        spot.SetActive(true);
    }

    private void TakeBackFromSpot(GameObject spot)
    {
        if (InventoryManager.Instance.IsFull())
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue(fullInventoryDialogue, false);
            }

            return;
        }

        if (InventoryManager.Instance.AddItem(candlestickId, candlestickIcon))
        {
            spot.SetActive(false);
        }
    }

    public int GetCandleCount()
    {
        int count = 0;

        if (leftSpot != null && leftSpot.activeSelf) count++;
        if (middleSpot != null && middleSpot.activeSelf) count++;
        if (rightSpot != null && rightSpot.activeSelf) count++;

        return count;
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

            EndSpotSelection();
        }
    }
}