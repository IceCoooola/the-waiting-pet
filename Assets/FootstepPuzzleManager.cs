using UnityEngine;

public class FootstepPuzzleManager : MonoBehaviour
{
    public RotatingPuzzlePiece[] puzzlePieces;

    public GameObject objectToReveal;
    private bool puzzleSolved = false;

    public void CheckPuzzle()
    {
        if (puzzleSolved) return;

        foreach (RotatingPuzzlePiece piece in puzzlePieces)
        {
            if (piece == null) continue;

            if (!piece.IsCorrect())
            {
                return;
            }
        }

        PuzzleSolved();
    }

    private void PuzzleSolved()
    {
        puzzleSolved = true;
        Debug.Log("Footstep puzzle solved!");

        if (objectToReveal != null)
        {
            objectToReveal.SetActive(true);
        }
    }
}