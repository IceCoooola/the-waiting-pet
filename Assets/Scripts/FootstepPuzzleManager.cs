using UnityEngine;

public class FootstepPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Pieces")]
    public RotatingPuzzlePiece[] puzzlePieces;

    [Header("Puzzle Piece Names")]
    public string[] puzzlePieceNames = { "Quill", "Bottle", "Glasses", "Rose" };

    [Header("Reward")]
    public GameObject objectToReveal;

    [Header("Texts")]
    [TextArea]
    public string lockedText = "There's some stuff on the table. Maybe it belongs to the owner.";

    [TextArea]
    public string firstInstruction = "The objects seem movable.\nPress SPACE to continue.";

    [TextArea]
    public string selectionInstruction =
        "Quill: Press 1\nBottle: Press 2\nGlasses: Press 3\nRose: Press 4";

    private int selectedIndex = 0;
    private bool playerInRange = false;
    private bool puzzleActive = false;
    private bool selectionMode = false;
    private bool puzzleSolved = false;

    private void Start()
    {
        if (objectToReveal != null)
        {
            objectToReveal.SetActive(false);
        }
    }

    private void Update()
    {
        if (!playerInRange) return;

        bool diaryFullyRead = GameProgress.Instance != null &&
                              GameProgress.Instance.diaryFullyRead;

        if (!diaryFullyRead)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ShowText(lockedText);
            }

            return;
        }

        if (!puzzleActive)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                puzzleActive = true;
                selectionMode = false;
                ShowText(firstInstruction);
            }

            return;
        }

        if (!selectionMode)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                selectionMode = true;
                ShowText(selectionInstruction);
            }

            return;
        }

        if (puzzleSolved) return;

        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectPiece(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) SelectPiece(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) SelectPiece(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) SelectPiece(3);

        if (Input.GetKeyDown(KeyCode.T))
        {
            RotateSelectedPiece();
            CheckPuzzle();
        }
    }

    private void SelectPiece(int index)
    {
        if (index < 0 || index >= puzzlePieces.Length) return;
        if (puzzlePieces[index] == null) return;

        selectedIndex = index;

        string pieceName = GetPieceName(index);

        ShowText("Selected: " + pieceName + "\nPress T to rotate.");
        Debug.Log("Selected object " + (index + 1) + ": " + pieceName);
    }

    private string GetPieceName(int index)
    {
        if (puzzlePieceNames != null &&
            index >= 0 &&
            index < puzzlePieceNames.Length &&
            !string.IsNullOrEmpty(puzzlePieceNames[index]))
        {
            return puzzlePieceNames[index];
        }

        return puzzlePieces[index].name;
    }

    private void RotateSelectedPiece()
    {
        if (selectedIndex < 0 || selectedIndex >= puzzlePieces.Length) return;
        if (puzzlePieces[selectedIndex] == null) return;

        puzzlePieces[selectedIndex].RotatePiece();

        string pieceName = GetPieceName(selectedIndex);
        ShowText("Selected: " + pieceName + "\nPress T to rotate.");
    }

    private void CheckPuzzle()
    {
        foreach (RotatingPuzzlePiece piece in puzzlePieces)
        {
            if (piece == null) return;

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

        ShowText("Something appeared on the carpet.");

        if (objectToReveal != null)
        {
            objectToReveal.SetActive(true);
        }

        Debug.Log("Puzzle solved!");
    }

    private void ShowText(string text)
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowDialogue(text, false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            bool diaryFullyRead = GameProgress.Instance != null &&
                                  GameProgress.Instance.diaryFullyRead;

            if (!puzzleSolved && diaryFullyRead)
            {
                
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            puzzleActive = false;
            selectionMode = false;

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.HideDialogue();
            }
        }
    }
}