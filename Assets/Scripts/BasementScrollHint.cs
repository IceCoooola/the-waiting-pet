using UnityEngine;

public class BasementScrollHint : MonoBehaviour
{
    public static bool ScrollEventTriggered = false; // shared across all scrolls

    private bool playerNear = false;
    private bool shown      = false;

    void Awake()
    {
        var col = GetComponent<Collider2D>() ?? gameObject.AddComponent<BoxCollider2D>();
        if (col is BoxCollider2D b) b.size = new Vector2(1.8f, 1.8f);
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) playerNear = true; }
    void OnTriggerExit2D(Collider2D other)  { if (other.CompareTag("Player")) playerNear = false; }

    void Update()
    {
        if (!playerNear || shown) return;
        if (BasementDialogueSequence.Instance != null && BasementDialogueSequence.Instance.IsPlaying) return;
        if (!Input.GetKeyDown(KeyCode.Space)) return;

        shown = true;
        ScrollEventTriggered = true;
        BasementDialogueSequence.Instance?.Play(new[] {
            "The green fire blocks the way, what was I first..."
        });
    }
}
