using UnityEngine;
using System.Collections;

/// <summary>
/// Cauldron spell puzzle: player types 3-letter answer (dog).
/// On success, fades cauldron and clears the physical path blocker.
/// </summary>
public class CauldronPuzzleInteraction : MonoBehaviour
{
    [Header("Path Blocker")]
    public GameObject pathBlocker; // invisible wall blocking top-right

    [Header("Answer")]
    private string answer = "dog";
    private bool isPlayerNear = false;
    private bool solved = false;

    // UI state
    private bool inputActive = false;
    private string currentInput = "";
    private string feedbackMessage = "";
    private float feedbackTimer = 0f;

    private GUIStyle inputStyle;
    private GUIStyle labelStyle;
    private Rect windowRect = new Rect(0, 0, 320, 160);
    private SpriteRenderer sr;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        var col = GetComponent<Collider2D>();
        if (col == null) { col = gameObject.AddComponent<BoxCollider2D>(); col.isTrigger = true; }
        else col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) isPlayerNear = true; }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) { isPlayerNear = false; inputActive = false; }
    }

    void Update()
    {
        if (solved || inputActive) return;
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Space))
        {
            inputActive = true;
            currentInput = "";
            feedbackMessage = "";
            // Center window
            windowRect.x = (Screen.width  - windowRect.width)  * 0.5f;
            windowRect.y = (Screen.height - windowRect.height) * 0.5f;
        }
        if (feedbackTimer > 0f) feedbackTimer -= Time.deltaTime;
    }

    void OnGUI()
    {
        if (!inputActive || solved) return;

        if (inputStyle == null)
        {
            inputStyle = new GUIStyle(GUI.skin.textField)  { fontSize = 22, alignment = TextAnchor.MiddleCenter };
            labelStyle  = new GUIStyle(GUI.skin.label)     { fontSize = 18, alignment = TextAnchor.MiddleCenter };
        }

        windowRect = GUI.Window(9999, windowRect, DrawWindow, "Cauldron Spell");
    }

    void DrawWindow(int id)
    {
        GUILayout.Space(10);
        GUILayout.Label("Enter the three-letter spell:", labelStyle);
        GUILayout.Space(8);

        GUI.SetNextControlName("CauldronInput");
        string raw = GUILayout.TextField(currentInput, 3, inputStyle, GUILayout.Height(36));
        currentInput = raw.ToLower();
        GUI.FocusControl("CauldronInput");

        GUILayout.Space(8);
        if (GUILayout.Button("Cast", GUILayout.Height(32)))
            TryAnswer();

        if (feedbackTimer > 0f)
        {
            GUI.color = Color.red;
            GUILayout.Label(feedbackMessage, labelStyle);
            GUI.color = Color.white;
        }
    }

    void TryAnswer()
    {
        if (currentInput.Trim() == answer)
        {
            inputActive = false;
            solved = true;
            StartCoroutine(SolveCauldron());
        }
        else
        {
            feedbackMessage = "Incorrect spell... try again.";
            feedbackTimer   = 2.5f;
            currentInput    = "";
        }
    }

    IEnumerator SolveCauldron()
    {
        DialogueManager.Instance?.ShowDialogue("The cauldron's fire fades...", true);

        // Fade cauldron out
        float t = 0f;
        Color startCol = sr.color;
        while (t < 1.5f)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / 1.5f);
            sr.color = new Color(startCol.r, startCol.g, startCol.b, a);
            yield return null;
        }
        gameObject.SetActive(false);

        // Clear path blocker
        if (pathBlocker != null) pathBlocker.SetActive(false);
    }
}
