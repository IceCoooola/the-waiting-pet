using UnityEngine;
using System.Collections.Generic;

public class CandlestickPuzzleManager : MonoBehaviour
{
    public List<CandlestickHolder> holders;
    public GameObject hallwayKey;
    public string rewardDialogue = "A clicking sound echoes... something appears in the hallway.";

    private bool puzzleSolved = false;

    private void Start()
    {
        if (hallwayKey != null)
        {
            hallwayKey.SetActive(false);
        }

        // Ensure all holders start empty
        if (holders != null)
        {
            foreach (var holder in holders)
            {
                if (holder == null) continue;
                if (holder.leftSpot != null) holder.leftSpot.SetActive(false);
                if (holder.middleSpot != null) holder.middleSpot.SetActive(false);
                if (holder.rightSpot != null) holder.rightSpot.SetActive(false);
            }
        }
    }

    public bool CheckPuzzle()
    {
        if (puzzleSolved) return false;
        if (holders == null || holders.Count < 4) return false;

        // candlesticks_empty (Index 0): Should be empty (0 candles)
        bool empty_0_correct = holders[0].GetCandleCount() == 0;

        // candlesticks_empty_1 (Index 1): Left, Middle, Right all active (3 candles)
        bool empty_1_correct = IsSpotActive(holders[1].leftSpot) && 
                              IsSpotActive(holders[1].middleSpot) && 
                              IsSpotActive(holders[1].rightSpot);

        // candlesticks_empty_2 (Index 2): Only Middle active
        bool empty_2_correct = !IsSpotActive(holders[2].leftSpot) && 
                               IsSpotActive(holders[2].middleSpot) && 
                               !IsSpotActive(holders[2].rightSpot);

        // candlesticks_empty_3 (Index 3): Should have 3 candles
        bool empty_3_correct = IsSpotActive(holders[3].leftSpot) && 
                               IsSpotActive(holders[3].middleSpot) && 
                               IsSpotActive(holders[3].rightSpot);

        if (empty_0_correct && empty_1_correct && empty_2_correct && empty_3_correct)
        {
            SolvePuzzle();
            return true;
        }

        return false;
    }

    private bool IsSpotActive(GameObject spot)
    {
        return spot != null && spot.activeSelf;
    }

    private void SolvePuzzle()
    {
        puzzleSolved = true;
        if (hallwayKey != null)
        {
            hallwayKey.SetActive(true);
        }

        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowDialogue(rewardDialogue);
        }
    }
}
