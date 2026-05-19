using UnityEngine;

/// <summary>
/// Floating bone pickup. On Space interact: adds 'bone' to InventoryManager.
/// </summary>
public class Bone_Item : MonoBehaviour
{
    public Sprite boneIcon; // assign in inspector or leave null
    private bool isPlayerNear = false;
    private bool pickedUp = false;

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
        if (pickedUp || !isPlayerNear) return;
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (InventoryManager.Instance == null) return;
            var icon = boneIcon != null ? boneIcon : GetComponent<SpriteRenderer>()?.sprite;
            if (InventoryManager.Instance.AddItem("bone", icon))
            {
                pickedUp = true;
                DialogueManager.Instance?.ShowDialogue("You picked up the bone.");
                gameObject.GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}
