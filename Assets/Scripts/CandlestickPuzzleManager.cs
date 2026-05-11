using UnityEngine;
using System.Collections.Generic;

public class CandlestickPuzzleManager : MonoBehaviour
{
    public List<CandlestickHolder> holders;
    public GameObject hallwayKey;
    public string rewardDialogue = "Something appeared in the hallway...";

    private bool puzzleSolved = false;

    public void CheckPuzzle()
    {
        if (puzzleSolved) return;
        if (holders == null || holders.Count < 4) return;

        // candlesticks_empty (Index 0): Should be empty (0 candles)
        bool empty_0_correct = holders[0].GetCandleCount() == 0;

        // candlesticks_empty_1 (Index 1): Left, Middle, Right all active
        bool empty_1_correct = IsSpotActive(holders[1].leftSpot) && 
                              IsSpotActive(holders[1].middleSpot) && 
                              IsSpotActive(holders[1].rightSpot);

        // candlesticks_empty_2 (Index 2): Only Middle active
        bool empty_2_correct = !IsSpotActive(holders[2].leftSpot) && 
                               IsSpotActive(holders[2].middleSpot) && 
                               !IsSpotActive(holders[2].rightSpot);

        // candlesticks_empty_3 (Index 3): Left and Right active
        bool empty_3_correct = IsSpotActive(holders[3].leftSpot) && 
                               !IsSpotActive(holders[3].middleSpot) && 
                               IsSpotActive(holders[3].rightSpot);

        if (empty_0_correct && empty_1_correct && empty_2_correct && empty_3_correct)
        {
            SolvePuzzle();
        }
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
