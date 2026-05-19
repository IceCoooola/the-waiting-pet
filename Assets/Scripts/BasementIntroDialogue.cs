using UnityEngine;
using System.Collections;

/// <summary>Plays the fish intro dialogue sequence on basement entry.</summary>
public class BasementIntroDialogue : MonoBehaviour
{
    private string[] lines = {
        "I can breathe underwater... I am now a fish.",
        "An orb... let me check it out."
    };
    private int currentLine = 0;
    private bool active = false;

    void Start() => StartCoroutine(ShowIntro());

    IEnumerator ShowIntro()
    {
        yield return new WaitForSeconds(0.5f);
        ShowLine(0);
    }

    void ShowLine(int idx)
    {
        currentLine = idx;
        active = true;
        DialogueManager.Instance?.ShowDialogue(lines[idx], false);
    }

    void Update()
    {
        if (!active) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            int next = currentLine + 1;
            if (next < lines.Length) ShowLine(next);
            else { active = false; DialogueManager.Instance?.HideDialogue(); }
        }
    }
}
