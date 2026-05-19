using UnityEngine;
using System.Collections;
using System;

/// <summary>Shows a sequence of dialogue lines, space to advance. Calls onComplete when done.</summary>
public class BasementSequenceDialogue : MonoBehaviour
{
    private string[] lines;
    private int idx = 0;
    private bool active = false;
    private Action onComplete;

    public void Play(string[] dialogueLines, Action callback = null)
    {
        lines = dialogueLines;
        idx = 0;
        onComplete = callback;
        active = true;
        DialogueManager.Instance?.ShowDialogue(lines[0], false);
    }

    void Update()
    {
        if (!active) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            idx++;
            if (idx < lines.Length)
                DialogueManager.Instance?.ShowDialogue(lines[idx], false);
            else
            {
                active = false;
                DialogueManager.Instance?.HideDialogue();
                onComplete?.Invoke();
            }
        }
    }
}
