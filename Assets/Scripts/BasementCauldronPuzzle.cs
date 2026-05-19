using UnityEngine;
using System.Collections;

public class BasementCauldronPuzzle : MonoBehaviour
{
    [Header("Path Blocker")]
    public GameObject pathBlocker;

    private readonly string answer = "dog";
    private bool playerNear = false;
    private bool solved     = false;
    private bool inputOpen  = false;
    private string userInput = "";
    private string feedback  = "";
    private float  feedTimer = 0f;
    private Rect   winRect   = new Rect(0, 0, 340, 200);

    // UI styles - pixel art aesthetic matching reference
    private GUIStyle panelStyle, titleStyle, inputStyle, labelStyle, btnStyle, errorStyle;

    void Awake()
    {
        var col = GetComponent<Collider2D>() ?? gameObject.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) playerNear = true; }
    void OnTriggerExit2D(Collider2D other)  { if (other.CompareTag("Player")) { playerNear = false; inputOpen = false; } }

    void Update()
    {
        if (solved || inputOpen) return;
        if (!playerNear) return;
        if (BasementDialogueSequence.Instance != null && BasementDialogueSequence.Instance.IsPlaying) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!BasementScrollHint.ScrollEventTriggered)
            {
                BasementDialogueSequence.Instance?.Play(new[] {
                    "Something blocks my path... maybe there's a clue nearby."
                });
                return;
            }
            inputOpen = true;
            userInput = "";
            feedback  = "";
            winRect.x = (Screen.width  - winRect.width)  * 0.5f;
            winRect.y = (Screen.height - winRect.height)  * 0.5f;
        }
        if (feedTimer > 0f) feedTimer -= Time.deltaTime;
    }

    void OnGUI()
    {
        if (!inputOpen || solved) return;
        InitStyles();

        // Dark overlay
        GUI.color = new Color(0f, 0f, 0f, 0.5f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        GUI.color = Color.white;

        winRect = GUI.Window(7777, winRect, DrawWindow, "", panelStyle);
    }

    void InitStyles()
    {
        if (panelStyle != null) return;

        // Panel: dark navy matching reference
        panelStyle = new GUIStyle(GUI.skin.box);
        panelStyle.normal.background = MakeTex(new Color(0.05f, 0.06f, 0.18f, 0.96f));
        panelStyle.border = new RectOffset(4,4,4,4);

        titleStyle = new GUIStyle(GUI.skin.label)
            { fontSize = 20, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter,
              normal = { textColor = new Color(0.4f,0.75f,1f) } };

        labelStyle = new GUIStyle(GUI.skin.label)
            { fontSize = 16, alignment = TextAnchor.MiddleCenter,
              normal = { textColor = Color.white } };

        inputStyle = new GUIStyle(GUI.skin.textField)
            { fontSize = 28, alignment = TextAnchor.MiddleCenter,
              fontStyle = FontStyle.Bold };

        btnStyle = new GUIStyle(GUI.skin.button)
            { fontSize = 16, fontStyle = FontStyle.Bold };
        btnStyle.normal.background   = MakeTex(new Color(0.2f, 0.4f, 0.8f, 1f));
        btnStyle.normal.textColor    = Color.white;
        btnStyle.hover.background    = MakeTex(new Color(0.3f, 0.55f, 1f, 1f));
        btnStyle.hover.textColor     = Color.white;

        errorStyle = new GUIStyle(GUI.skin.label)
            { fontSize = 14, alignment = TextAnchor.MiddleCenter,
              normal = { textColor = new Color(1f, 0.4f, 0.4f) } };
    }

    Texture2D MakeTex(Color c)
    {
        var t = new Texture2D(1,1);
        t.SetPixel(0,0,c); t.Apply();
        return t;
    }

    void DrawWindow(int id)
    {
        GUILayout.Space(12);
        GUILayout.Label("Witch's Cauldron", titleStyle);
        GUILayout.Space(6);
        GUILayout.Label("Speak the 3-letter word to pass:", labelStyle);
        GUILayout.Space(8);
        GUI.SetNextControlName("SpellInput");
        userInput = GUILayout.TextField(userInput, 3, inputStyle, GUILayout.Height(44)).ToLower();
        GUI.FocusControl("SpellInput");
        GUILayout.Space(8);
        if (GUILayout.Button("Cast Spell", btnStyle, GUILayout.Height(36))) TryAnswer();
        if (feedTimer > 0f) GUILayout.Label(feedback, errorStyle);
        GUILayout.Space(8);
    }

    void TryAnswer()
    {
        if (userInput.Trim() == answer) { inputOpen = false; solved = true; StartCoroutine(Solve()); }
        else { feedback = "That's not right... try again."; feedTimer = 2.5f; userInput = ""; }
    }

    IEnumerator Solve()
    {
        BasementDialogueSequence.Instance?.Play(new[] { "The green fire fades..." });
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) {
            float e = 0f; Color c = sr.color;
            while (e < 1.5f) { e += Time.deltaTime; sr.color = new Color(c.r,c.g,c.b, Mathf.Lerp(1f,0f,e/1.5f)); yield return null; }
        }
        foreach (var csr in GetComponentsInChildren<SpriteRenderer>()) {
            var c2 = csr.color; csr.color = new Color(c2.r,c2.g,c2.b,0f);
        }
        if (pathBlocker != null) pathBlocker.SetActive(false);
        gameObject.SetActive(false);
    }
}
