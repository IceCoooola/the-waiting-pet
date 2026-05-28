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

    [Header("Hint")]
    [TextArea]
    public string hintText = "Hmm... The diary says the wider side is the direction I move forward... How does that relate to the objects on the table?";
    public float hintDelay = 180f;
    private float hintTimer = 0f;
    private bool hintShown = false;

    private int selectedIndex = 0;
    private bool playerInRange = false;
    private bool puzzleActive = false;
    private bool selectionMode = false;

    private void Start()
    {
        ApplyState();
    }

    private void OnEnable()
    {
        ApplyState();
    }

    private void ApplyState()
    {
        if (GameData.FootstepPuzzleSolved)
        {
            if (objectToReveal != null) objectToReveal.SetActive(true);
            return;
        }

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

        if (!GameData.FootstepPuzzleSolved && diaryFullyRead && !hintShown)
        {
            hintTimer += Time.deltaTime;
            if (hintTimer >= hintDelay)
            {
                ShowText(hintText);
                hintShown = true;
            }
        }

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

        if (GameData.FootstepPuzzleSolved) return;

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

        ShowText("Selected: " + pieceName + "\nPress T to rotate.\nPress 1, 2, 3, 4 to reselect the item.");
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
        ShowText("Selected: " + pieceName + "\nPress T to rotate.\nPress 1, 2, 3, 4 to reselect the item.");
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
        GameData.FootstepPuzzleSolved = true;
        ApplyState();

        ShowText("Something appeared on the carpet.");

        Debug.Log("Puzzle solved!");
    }

    private void ShowText(string text)
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowDialogue(text, false, 0, null, false, true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            bool diaryFullyRead = GameProgress.Instance != null &&
                                  GameProgress.Instance.diaryFullyRead;

            if (!GameData.FootstepPuzzleSolved && diaryFullyRead)
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