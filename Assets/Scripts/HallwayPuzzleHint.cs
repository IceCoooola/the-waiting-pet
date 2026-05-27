using UnityEngine;
using System.Collections;

public class HallwayPuzzleHint : MonoBehaviour
{
    public float hintDelay = 180f; // 3 minutes
    public string hintDialogue = "Some barrels are standing up, some are lying down... Does that relate to the candle placement?";
    
    private CandlestickPuzzleManager puzzleManager;
    private Coroutine hintCoroutine;
    private bool hintShown = false;

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
        if (hintShown) return;
        
        StopHintCoroutine();
        hintCoroutine = StartCoroutine(HintTimerRoutine());
    }

    private void OnDisable()
    {
        StopHintCoroutine();
    }

    private void StopHintCoroutine()
    {
        if (hintCoroutine != null)
        {
            StopCoroutine(hintCoroutine);
            hintCoroutine = null;
        }
    }

    private IEnumerator HintTimerRoutine()
    {
        yield return new WaitForSeconds(hintDelay);

        if (puzzleManager != null && !puzzleManager.IsSolved)
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue(hintDialogue, false);
                hintShown = true;
            }
        }
        
        hintCoroutine = null;
    }
}
