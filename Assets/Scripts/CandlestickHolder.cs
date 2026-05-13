using UnityEngine;

public class CandlestickHolder : MonoBehaviour
{
    private enum InteractionStep { Idle, FirstPrompt, SlotSelection }
    private InteractionStep currentStep = InteractionStep.Idle;

    [Header("Spots")]
    public GameObject leftSpot;
    public GameObject middleSpot;
    public GameObject rightSpot;

    [Header("Item Info")]
    public string candlestickId = "Candlestick";
    public Sprite candlestickIcon;

    private bool isPlayerInRange;
    private float choiceTimer;
    private const float ChoiceDuration = 5f;

    private void Update()
    {
        if (!isPlayerInRange) return;

        if (Input.GetKeyDown(KeyCode.Space) && currentStep == InteractionStep.Idle)
        {
            if (InventoryManager.Instance != null && InventoryManager.Instance.CanInteract())
            {
                StartInteraction();
            }
        }

        if (currentStep != InteractionStep.Idle)
        {
            choiceTimer -= Time.deltaTime;
            if (choiceTimer <= 0)
            {
                EndInteraction();
                if (DialogueManager.Instance != null) DialogueManager.Instance.HideDialogue();
                return;
            }

            bool hasHand = InventoryManager.Instance != null && InventoryManager.Instance.HasItem(candlestickId);
            int candleCount = GetCandleCount();

            if (currentStep == InteractionStep.FirstPrompt)
            {
                if (hasHand && candleCount > 0) // Case 3: Choices 1 or 2
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        ShowSlotSelection();
                    }
                }
                else // Case 2 or 4: Press Space
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        ShowSlotSelection();
                    }
                }
            }
            else if (currentStep == InteractionStep.SlotSelection)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1)) HandleSpotInteraction(leftSpot);
                else if (Input.GetKeyDown(KeyCode.Alpha2)) HandleSpotInteraction(middleSpot);
                else if (Input.GetKeyDown(KeyCode.Alpha3)) HandleSpotInteraction(rightSpot);
            }
        }
    }

    private void StartInteraction()
    {
        bool hasHand = InventoryManager.Instance != null && InventoryManager.Instance.HasItem(candlestickId);
        int candleCount = GetCandleCount();

        if (DialogueManager.Instance == null) return;

        if (!hasHand && candleCount == 0) // Case 1
        {
            DialogueManager.Instance.ShowDialogue("Empty candlestick...");
            return;
        }

        choiceTimer = ChoiceDuration;
        currentStep = InteractionStep.FirstPrompt;

        if (hasHand && candleCount == 0) // Case 2
        {
            DialogueManager.Instance.ShowDialogue("Place the candle in the candlestick.", false);
        }
        else if (hasHand && candleCount > 0) // Case 3
        {
            DialogueManager.Instance.ShowDialogue("Take the candle back \n(Press 1)\nPlace the candle \n(Press 2)", false);
        }
        else if (!hasHand && candleCount > 0) // Case 4
        {
            DialogueManager.Instance.ShowDialogue("Take the candle back from the candlestick", false);
        }
    }

    private void ShowSlotSelection()
    {
        bool hasHand = InventoryManager.Instance != null && InventoryManager.Instance.HasItem(candlestickId);
        int candleCount = GetCandleCount();
        choiceTimer = ChoiceDuration;
        currentStep = InteractionStep.SlotSelection;

        if (DialogueManager.Instance == null) return;

        if (hasHand && candleCount == 0) // Case 2
        {
            DialogueManager.Instance.ShowDialogue("Place it on the left.\n(Press 1)\nPlace it on the middle.\n(Press 2)\nPlace it on the right\n(Press 3)", false);
        }
        else // Case 3 or 4
        {
            DialogueManager.Instance.ShowDialogue("Take the left back.\n(Press 1)\nTake the middle back.\n(Press 2)\nTake the right back.\n(Press 3)", false);
        }
    }

    private void HandleSpotInteraction(GameObject spot)
    {
        if (spot == null) return;

        bool actionTaken = false;

        if (!spot.activeSelf)
        {
            // Try to place
            if (InventoryManager.Instance != null && InventoryManager.Instance.HasItem(candlestickId))
            {
                InventoryManager.Instance.RemoveItem(candlestickId);
                spot.SetActive(true);
                if (DialogueManager.Instance != null) DialogueManager.Instance.ShowDialogue("Placed candlestick.");
                actionTaken = true;
            }
            else
            {
                if (DialogueManager.Instance != null) DialogueManager.Instance.ShowDialogue("I need a candlestick.");
            }
        }
        else
        {
            // Try to pick back
            if (InventoryManager.Instance != null)
            {
                if (InventoryManager.Instance.IsFull())
                {
                    if (DialogueManager.Instance != null) DialogueManager.Instance.ShowDialogue("I can't carry it, that's too many");
                    EndInteraction();
                    return;
                }

                if (InventoryManager.Instance.AddItem(candlestickId, candlestickIcon))
                {
                    spot.SetActive(false);
                    if (DialogueManager.Instance != null) DialogueManager.Instance.ShowDialogue("Took back candlestick.");
                    actionTaken = true;
                }
            }
        }

        if (actionTaken)
        {
            CandlestickPuzzleManager manager = Object.FindFirstObjectByType<CandlestickPuzzleManager>();
            if (manager != null) manager.CheckPuzzle();
        }

        EndInteraction();
    }

    private void EndInteraction()
    {
        currentStep = InteractionStep.Idle;
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
        if (other.CompareTag("Player")) isPlayerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            EndInteraction();
            if (DialogueManager.Instance != null) DialogueManager.Instance.HideDialogue();
        }
    }
}

