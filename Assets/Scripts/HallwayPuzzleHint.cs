using UnityEngine;

public class HallwayPuzzleHint : MonoBehaviour
{
    public float hintDelay = 180f; // 3 minutes
    public string hintDialogue = "Some barrels are standing up, some are lying down... Does that relate to the candle placement?";
    
    private CandlestickPuzzleManager puzzleManager;
    private float hintTimer = 0f;
    private bool hintShown = false;
    private bool playerInRange = false;

    private void Awake()
    {
        puzzleManager = GetComponentInChildren<CandlestickPuzzleManager>();
        if (puzzleManager == null)
        {
            puzzleManager = Object.FindAnyObjectByType<CandlestickPuzzleManager>();
        }
    }

    private void OnEnable()
    {
        playerInRange = false;
        hintTimer = 0f;
    }

    private void OnDisable()
    {
        playerInRange = false;
    }

    private void Update()
    {
        if (hintShown || !playerInRange) return;

        if (puzzleManager != null && !puzzleManager.IsSolved)
        {
            hintTimer += Time.deltaTime;
            if (hintTimer >= hintDelay)
            {
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.ShowDialogue(hintDialogue, false);
                    hintShown = true;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            hintTimer = 0f;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            hintTimer = 0f;
        }
    }
}
