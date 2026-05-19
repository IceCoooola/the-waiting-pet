using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Single-instance dialogue sequencer for the basement.
/// Call Play() with lines + optional callback. Space advances.
/// </summary>
public class BasementDialogueSequence : MonoBehaviour
{
    public static BasementDialogueSequence Instance;

    private Queue<string> queue = new Queue<string>();
    private bool playing = false;
    private Action onDone;
    // Skip Space for one frame after Play() starts so the keydown that
    // triggered Play() cannot also immediately advance line 1.
    private int _skipFrames = 0;


    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

void Update()
    {
        if (!playing) return;
        if (_skipFrames > 0) { _skipFrames--; return; }
        if (Input.GetKeyDown(KeyCode.Space)) Advance();
    }

public void Play(string[] lines, Action callback = null)
    {
        StopAllCoroutines();
        queue.Clear();
        foreach (var l in lines) queue.Enqueue(l);
        onDone      = callback;
        playing     = true;
        _skipFrames = 1;
        ShowNext();
    }

    public bool IsPlaying => playing;

    void ShowNext()
    {
        if (queue.Count > 0)
            DialogueManager.Instance?.ShowDialogue(queue.Dequeue(), false);
        else End();
    }

    void Advance()
    {
        if (queue.Count > 0) ShowNext();
        else End();
    }

    void End()
    {
        playing = false;
        DialogueManager.Instance?.HideDialogue();
        var cb = onDone; onDone = null;
        cb?.Invoke();
    }
}
