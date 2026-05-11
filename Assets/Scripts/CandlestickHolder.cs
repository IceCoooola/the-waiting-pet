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

    private bool isPlayerInRange;
    private bool isChoosing;
    private float choiceTimer;
    private const float ChoiceDuration = 3f;

    private void Update()
    {
        if (!isPlayerInRange) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (InventoryManager.Instance != null && InventoryManager.Instance.CanInteract())
            {
                isChoosing = true;
                choiceTimer = ChoiceDuration;
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.ShowDialogue("Press 1: Left, 2: Middle, 3: Right");
                }
            }
        }

        if (isChoosing)
        {
            choiceTimer -= Time.deltaTime;
            if (choiceTimer <= 0)
            {
                isChoosing = false;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1)) HandleSpotInteraction(leftSpot);
            else if (Input.GetKeyDown(KeyCode.Alpha2)) HandleSpotInteraction(middleSpot);
            else if (Input.GetKeyDown(KeyCode.Alpha3)) HandleSpotInteraction(rightSpot);
        }
    }

    private void HandleSpotInteraction(GameObject spot)
    {
        if (spot == null) return;

        isChoosing = false; // Choice made

        if (!spot.activeSelf)
        {
            // Try to place
            if (InventoryManager.Instance != null && InventoryManager.Instance.HasItem(candlestickId))
            {
                InventoryManager.Instance.RemoveItem(candlestickId);
                spot.SetActive(true);
                if (DialogueManager.Instance != null) DialogueManager.Instance.ShowDialogue("Placed candlestick.");
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
                    return;
                }

                if (InventoryManager.Instance.AddItem(candlestickId, candlestickIcon))
                {
                    spot.SetActive(false);
                    if (DialogueManager.Instance != null) DialogueManager.Instance.ShowDialogue("Took back candlestick.");
                }
            }
        }

        // Notify Puzzle Manager
        CandlestickPuzzleManager manager = Object.FindFirstObjectByType<CandlestickPuzzleManager>();
        if (manager != null) manager.CheckPuzzle();
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
            isChoosing = false;
        }
    }
}
