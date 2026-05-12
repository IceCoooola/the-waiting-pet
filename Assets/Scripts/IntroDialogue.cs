using UnityEngine;

public class IntroDialogue : MonoBehaviour
{
    private string[] sentences = new string[]
    {
        "I'm hungry..\n(Press Space to continue)",
        "who am I..\n(Press Space to continue)",
        "ummm I looks like someone's pet\n(Press Space to continue)",
        "but why they didn't feed me..?\n(Press Space to continue)",
        "I want to find some food..\n(Press Space to continue)"
    };

    private int currentIndex = 0;
    private bool isFinished = false;

    private void Start()
    {
        // Disable player movement at start
        SetPlayerMovement(false);
        ShowCurrentSentence();
    }

    private void Update()
    {
        if (isFinished) return;

        // Check for Space key to advance
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentIndex++;
            if (currentIndex < sentences.Length)
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
            // Show dialogue without auto-hiding
            DialogueManager.Instance.ShowDialogue(sentences[currentIndex], false);
        }
    }

    private void FinishDialogue()
    {
        isFinished = true;
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.HideDialogue();
        }

        // Re-enable player movement
        SetPlayerMovement(true);
        
        // Remove this script after completion
        Destroy(this);
    }

    private void SetPlayerMovement(bool state)
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            var movement = player.GetComponent<PlayerMovement>();
            if (movement != null)
            {
                movement.enabled = state;
                
                // Also stop the player if they were somehow moving
                var rb = player.GetComponent<Rigidbody2D>();
                if (rb != null) rb.linearVelocity = Vector2.zero;
            }
        }
    }
}