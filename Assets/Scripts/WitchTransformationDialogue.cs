using UnityEngine;

public class WitchTransformationDialogue : MonoBehaviour
{
    private string[] sentences = new string[]
    {
        "I remember everything now.\n(Press space to continue.)",
        "The fire. The smoke. The screams outside our house.\n(Press space to continue.)",
        "They called us witches like it was something rotten, something evil. The villagers dragged my mother and my grandmother inside and burned them alive while the church bells rang in the distance. I can still hear them screaming my name.\n(Press space to continue.)",
        "My mother hid me in the cellar before they came. She told me not to cry. Not to move. No matter what happened upstairs.\n(Press space to continue.)",
        "They locked every door in the house… but they forgot the cellar hatch.\n(Press space to continue.)",
        "So I ran.\n(Press space to continue.)",
        "For years, I told myself I was only surviving. That I was only hiding.\n(Press space to continue.)",
        "But now I remember.\n(Press space to continue.)",
        "I am my mother’s daughter.\nMy grandmother’s blood runs through me.\nThe fire did not kill us.\n(Press space to continue.)",
        "And if the hunters are coming again…\n(Press space to continue.)",
        "This time, I will not run.\n(Press space to continue.)"
    };

    private int currentIndex = 0;
    private bool isFinished = false;

    private void Start()
    {
        SetPlayerMovement(false);
        ShowCurrentSentence();
    }

    private void Update()
    {
        if (isFinished) return;

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

        SetPlayerMovement(true);
        Destroy(this);
    }

    private void SetPlayerMovement(bool state)
    {
        var movement = GetComponent<PlayerMovement>();
        if (movement != null)
        {
            movement.enabled = state;
            var rb = GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }
    }
}
