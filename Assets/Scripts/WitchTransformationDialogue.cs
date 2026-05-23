using UnityEngine;

public class WitchTransformationDialogue : MonoBehaviour
{
    [System.Serializable]
    public struct DialogueLine
    {
        public string portraitName;
        public string text;
    }

private DialogueLine[] lines = new DialogueLine[]
{
    new DialogueLine { portraitName = "", text = "I remember everything now.\n(Press space to continue.)" },
    new DialogueLine { portraitName = "witch 1", text = "The fire. The smoke. The chaos outside our house.\n(Press space to continue.)" },
    new DialogueLine { portraitName = "witch 2", text = "They called us witches like it was something terrible, something wrong. The villagers came for my mother and my grandmother while the church bells rang in the distance. I can still hear them calling my name.\n(Press space to continue.)" },
    new DialogueLine { portraitName = "witch 3", text = "My mother hid me in the cellar before they arrived. She told me not to cry. Not to move. No matter what happened upstairs.\n(Press space to continue.)" },
    new DialogueLine { portraitName = "witch 4", text = "They locked every door in the house… but they forgot the cellar hatch.\n(Press space to continue.)" },
    new DialogueLine { portraitName = "witch 4", text = "So I ran.\n(Press space to continue.)" },
    new DialogueLine { portraitName = "witch 5", text = "For years, I told myself I was only surviving. That I was only hiding.\n(Press space to continue.)" },
    new DialogueLine { portraitName = "witch 5", text = "But now I remember.\n(Press space to continue.)" },
    new DialogueLine { portraitName = "witch 7", text = "I am my mother's daughter.\nMy grandmother's strength lives in me.\nThey could not take everything from us.\n(Press space to continue.)" },
    new DialogueLine { portraitName = "witch 7", text = "And if the hunters are coming again…\n(Press space to continue.)" },
    new DialogueLine { portraitName = "witch 7", text = "This time, I will not run.\n(Press space to continue.)" }
};

    private int currentIndex = 0;
    private bool isFinished = false;

    private void Start()
    {
        SetPlayerMovement(false);
        HideOtherUI();
        ShowCurrentSentence();
    }

    private void HideOtherUI()
    {
        // Disable all other canvases entirely
        Canvas[] canvases = GameObject.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        foreach (var canvas in canvases)
        {
            if (canvas.name != "DialogueCanvas")
            {
                canvas.gameObject.SetActive(false);
            }
            else
            {
                // In DialogueCanvas, hide everything except the DialoguePanel
                // We use GetComponentsInChildren to find things even if they are nested
                foreach (Transform child in canvas.transform)
                {
                    if (child.name != "DialoguePanel")
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }
        }
        
        // Final sweep for common HUD elements by name if they are on the same canvas
        string[] targets = { "PotionPanel", "InventoryGrid", "ItemGrid", "HUD", "PlayerStats", "ManualPanel" };
        foreach (string target in targets)
        {
            GameObject go = GameObject.Find(target);
            if (go != null) go.SetActive(false);
        }
    }

    private void Update()
    {
        if (isFinished)
        {
            // Just block everything at the end
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentIndex++;
            if (currentIndex < lines.Length)
            {
                ShowCurrentSentence();
            }
            else
            {
                FinishDialogue();
            }
        }
    }

    private void ShowCurrentSentence()
    {
        if (DialogueManager.Instance != null)
        {
            Sprite portrait = null;
            if (!string.IsNullOrEmpty(lines[currentIndex].portraitName))
            {
                portrait = Resources.Load<Sprite>("Backgrounds/" + lines[currentIndex].portraitName);
            }
            // Increase font size for cutscene effect
            DialogueManager.Instance.ShowDialogue(lines[currentIndex].text, false, 28, portrait, true);
        }
    }

    private void FinishDialogue()
    {
        isFinished = true;
        
        if (DialogueManager.Instance != null)
        {
            // Final screen with larger text
            DialogueManager.Instance.ShowDialogue("THE END\n(Thank you for playing!)", false, 48, null, true);
        }
    }

    private void SetPlayerMovement(bool state)
    {
        var movement = GetComponent<PlayerMovement>();
        if (movement == null)
        {
            // Try to find it on the parent or nearby if this script was added to a child?
            // But it's added to the player object in CrystalBallDialogue
            movement = GetComponentInParent<PlayerMovement>();
        }

        if (movement != null)
        {
            movement.enabled = state;
            var rb = movement.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }
    }
}
