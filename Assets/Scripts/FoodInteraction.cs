using UnityEngine;

public class FoodInteraction : MonoBehaviour
{
    [Header("Boxes")]
    public GameObject emptyBox;
    
    [Header("Heart Visual")]
    public GameObject heartPrefab;
    public float heartDisplayDuration = 2f;
    public Vector3 heartOffset = new Vector3(0, 1.2f, 0);

    [Header("Settings")]
    public float interactionDistance = 5.0f;
    public KeyCode interactKey = KeyCode.Space;

    private GameObject player;
    private bool isEating = false;
    private int dialogueIndex = 0;
    private string[] dialogues = new string[]
    {
        "I saw some bread on top of the dining table...\n(Press Space to continue)",
        "I stepped on the chair and ate it.\n(Press Space to continue)",
        "Yummy!\n(Press Space to continue)",
        "Hmm... Why does this house have no pet food or pet bowls?\n(Press Space to continue)",
        "That's weird...\n(Press Space to continue)"
    };

    void Start()
    {
        // Find player by tag or name
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) player = GameObject.Find("GoldenRetrieverPlayer");

        // Ensure empty box is hidden at start
        if (emptyBox != null)
        {
            emptyBox.SetActive(false);
        }
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        bool inRange = distance <= interactionDistance;

        // Disable auto-hide while in dialogue to prevent interruptions
        if (!inRange && isEating)
        {
            if (distance > interactionDistance * 2f) 
            {
                isEating = false;
                if (DialogueManager.Instance != null)
                    DialogueManager.Instance.HideDialogue();
            }
        }

        if (Input.GetKeyDown(interactKey))
        {
            if (isEating)
            {
                // Advance dialogue
                dialogueIndex++;
                if (dialogueIndex < dialogues.Length)
                {
                    if (DialogueManager.Instance != null)
                        DialogueManager.Instance.ShowDialogue(dialogues[dialogueIndex], false, 0, null, false, true);
                }
                else
                {
                    isEating = false;
                    if (DialogueManager.Instance != null)
                        DialogueManager.Instance.HideDialogue();
                }
                return;
                }

                if (inRange)
                {
                // Check if something else already handled interaction this frame
                if (InventoryManager.Instance != null && !InventoryManager.Instance.CanInteract()) return;
                
                EatFood();
                }
                }
                }

                private void EatFood()
                {
                // 1. Hide visuals and disable collision so the object "disappears" but the script stays active
                var sr = GetComponent<SpriteRenderer>();
                if (sr != null) sr.enabled = false;
        
                var col2D = GetComponent<Collider2D>();
                if (col2D != null) col2D.enabled = false;

                // Fallback for 3D components if any
                var r = GetComponent<Renderer>();
                if (r != null && sr == null) r.enabled = false;
        
                var col = GetComponent<Collider>();
                if (col != null && col2D == null) col.enabled = false;

                if (emptyBox != null)
                {
                emptyBox.SetActive(true);
                }

                // 2. Start dialogue sequence
                isEating = true;
                dialogueIndex = 0;
                if (DialogueManager.Instance != null)
                {
                DialogueManager.Instance.ShowDialogue(dialogues[dialogueIndex], false, 0, null, false, true);
                }
                else
                {
            Debug.LogWarning("DialogueManager instance not found!");
        }

        // 3. Show heart
        if (heartPrefab != null)
        {
            GameObject heart = Instantiate(heartPrefab, player.transform.position + heartOffset, Quaternion.identity);
            heart.transform.SetParent(player.transform);
            Destroy(heart, heartDisplayDuration);
        }
    }
}
