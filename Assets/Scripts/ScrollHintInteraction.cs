using UnityEngine;

/// <summary>Floating scroll/letter - shows cauldron hint on Space.</summary>
public class ScrollHintInteraction : MonoBehaviour
{
    private bool isPlayerNear = false;

    void Awake()
    {
        var col = GetComponent<Collider2D>();
        if (col == null) { col = gameObject.AddComponent<BoxCollider2D>(); col.isTrigger = true; }
        else col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) isPlayerNear = true; }
    void OnTriggerExit2D(Collider2D other)  { if (other.CompareTag("Player")) isPlayerNear = false; }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.Space))
            DialogueManager.Instance?.ShowDialogue(
                "The green fire blocks the way, remember who you first were.");
    }
}
