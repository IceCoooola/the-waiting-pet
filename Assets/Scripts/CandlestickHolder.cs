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
    public string emptyDialogue = "Empty candlestick...";

    [TextArea]
    public string needCandlestickDialogue = "I need a candlestick.";

    [TextArea]
    public string fullInventoryDialogue = "I can't carry more.";

    private bool isPlayerInRange = false;
    private bool choosingSpot = false;

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
                ToggleSpot(leftSpot, "left");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                ToggleSpot(middleSpot, "middle");
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                ToggleSpot(rightSpot, "right");
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
                DialogueManager.Instance.ShowDialogue(emptyDialogue, true);
            }

            return;
        }

        choosingSpot = true;
        UpdateDialogue();
    }

    private void UpdateDialogue()
    {
        if (DialogueManager.Instance == null) return;

        bool hasHand = InventoryManager.Instance != null && InventoryManager.Instance.HasItem(candlestickId);
        string dialogue = "";

        dialogue += GetSpotOption(leftSpot, "left", "1", hasHand);
        dialogue += GetSpotOption(middleSpot, "middle", "2", hasHand);
        dialogue += GetSpotOption(rightSpot, "right", "3", hasHand);

        if (string.IsNullOrEmpty(dialogue))
        {
            dialogue = "Nothing more to do here.\n";
        }

        dialogue += "Press Space to close.";
        DialogueManager.Instance.ShowDialogue(dialogue, false);
    }

    private string GetSpotOption(GameObject spot, string spotName, string keyNum, bool hasHand)
    {
        if (spot == null) return "";

        if (spot.activeSelf)
        {
            return $"Take the {spotName} candle back (Press {keyNum})\n";
        }
        else if (hasHand)
        {
            return $"Place candle on the {spotName} (Press {keyNum})\n";
        }

        return "";
    }

    private void EndSpotSelection()
    {
        choosingSpot = false;

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.HideDialogue();
        }
    }

    private void ToggleSpot(GameObject spot, string spotName)
    {
        if (spot == null || !choosingSpot) return;
        if (InventoryManager.Instance == null) return;

        bool wasActive = spot.activeSelf;
        string feedback = "";

        if (wasActive)
        {
            if (InventoryManager.Instance.IsFull())
            {
                DialogueManager.Instance?.ShowDialogue(fullInventoryDialogue, true);
                choosingSpot = false;
                return;
            }

            if (InventoryManager.Instance.AddItem(candlestickId, candlestickIcon))
            {
                spot.SetActive(false);
                feedback = "Took the candle back.";
            }
        }
        else
        {
            if (!InventoryManager.Instance.HasItem(candlestickId))
            {
                DialogueManager.Instance?.ShowDialogue(needCandlestickDialogue, true);
                choosingSpot = false;
                return;
            }

            InventoryManager.Instance.RemoveItem(candlestickId);
            spot.SetActive(true);
            feedback = "Placed a candle.";
        }

        // Notify puzzle manager
        CandlestickPuzzleManager manager = Object.FindAnyObjectByType<CandlestickPuzzleManager>();
        bool solvedNow = false;
        if (manager != null)
        {
            solvedNow = manager.CheckPuzzle();
        }

        // Show feedback and end interaction
        choosingSpot = false;
        if (DialogueManager.Instance != null && !string.IsNullOrEmpty(feedback))
        {
            // Only show generic feedback if the puzzle wasn't just solved.
            // If it was solved, the manager already showed the reward dialogue.
            if (!solvedNow)
            {
                DialogueManager.Instance.ShowDialogue(feedback, true);
            }
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