using UnityEngine;

public class PotionCabinetInteraction : MonoBehaviour
{
    public float interactionDistance = 2.5f;
    [Tooltip("The cat animator used for the transformation.")]
    public RuntimeAnimatorController catAnimator;
    
    [Header("UI")]
    public string initialMessage = "Choose a potion: 1. Red 2. Blue 3. Transformation";
    public string transformMessage = "Poof! You are now a cat!";
    
    private Transform player;
    private bool isAsking = false;

    private void Start()
    {
        FindPlayer();
    }

    private void FindPlayer()
    {
        GameObject p = GameObject.Find("GoldenRetrieverPlayer");
        if (p == null) p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    private void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        bool inRange = distance <= interactionDistance;

        // Interaction prompt or automatic detection
        if (inRange && Input.GetKeyDown(KeyCode.Space) && !isAsking)
        {
            Debug.Log("[PotionCabinet] Player interacted with cabinet at " + transform.position);
            isAsking = true;
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue(initialMessage);
            }
        }

        if (isAsking)
        {
            // Potion 3: Transformation
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.Log("[PotionCabinet] Potion 3 chosen: Transforming...");
                TransformToCat();
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.ShowDialogue(transformMessage);
                }
                isAsking = false;
            }
            // Potion 1 & 2: Flavor text
            else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.Log("[PotionCabinet] Other potion chosen.");
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.ShowDialogue("Tastes like fizzy water.");
                }
                isAsking = false;
            }
            
            if (!inRange)
            {
                isAsking = false;
                if (DialogueManager.Instance != null) DialogueManager.Instance.HideDialogue();
            }
        }
    }

    private void TransformToCat()
    {
        if (player == null || catAnimator == null) 
        {
            Debug.LogWarning("[PotionCabinet] Missing player or catAnimator!");
            return;
        }
        
        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.runtimeAnimatorController = catAnimator;
            // Force an update to the animator to ensure it picks up the change
            anim.Update(0);
            Debug.Log("[PotionCabinet] Swapped animator controller to: " + catAnimator.name);
        }
        else
        {
            Debug.LogWarning("[PotionCabinet] Missing Animator on player!");
        }
        
        // Change the name for clarity (optional)
        player.name = "CatPlayer";
    }
}