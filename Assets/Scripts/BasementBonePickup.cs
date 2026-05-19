using UnityEngine;

/// <summary>
/// Space near bone -> add 'bone' to InventoryManager.
/// Hide bone sprite after pickup.
/// </summary>
public class BasementBonePickup : MonoBehaviour
{
    public Sprite boneIcon;
    private bool playerNear = false;
    private bool pickedUp   = false;

    void Awake()
    {
        var col = GetComponent<Collider2D>() ?? gameObject.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        if (boneIcon == null)
            boneIcon = GetComponent<SpriteRenderer>()?.sprite;
    }

    void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) playerNear = true; }
    void OnTriggerExit2D(Collider2D other)  { if (other.CompareTag("Player")) playerNear = false; }

    void Update()
    {
        if (pickedUp || !playerNear || BasementDialogueSequence.Instance.IsPlaying) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (InventoryManager.Instance == null) return;
            if (InventoryManager.Instance.AddItem("bone", boneIcon))
            {
                pickedUp = true;
                BasementDialogueSequence.Instance.Play(new[] { "You found a bone!" });
                var sr = GetComponent<SpriteRenderer>();
                if (sr != null) sr.enabled = false;
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}
