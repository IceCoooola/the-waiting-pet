using UnityEngine;

public class PotionCabinetInteraction : MonoBehaviour
{
    public float interactionDistance = 2.5f;
    public RuntimeAnimatorController catAnimator;

    [Header("Cat Transformation")]
    public GameObject ladderObject; // Ladder appears if player is a cat

    [Header("UI")]
    public string initialMessage = "Choose a potion: 1. Red 2. Blue 3. Transformation";
    public string transformMessage = "Poof! You are now a cat!";

    private Transform player;
    private bool isAsking = false;

    private void Start()
    {
        FindPlayer();

        if (ladderObject != null)
            ladderObject.SetActive(false);
    }

    private void FindPlayer()
    {
        GameObject p = GameObject.Find("GoldenRetrieverPlayer");
        if (p == null) p = GameObject.Find("CatPlayer");
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

        if (inRange && Input.GetKeyDown(KeyCode.Space) && !isAsking)
        {
            isAsking = true;

            if (DialogueManager.Instance != null)
                DialogueManager.Instance.ShowDialogue(initialMessage);
        }

        if (isAsking)
        {
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                TransformToCat();

                if (DialogueManager.Instance != null)
                    DialogueManager.Instance.ShowDialogue(transformMessage);

                isAsking = false;
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (DialogueManager.Instance != null)
                    DialogueManager.Instance.ShowDialogue("Tastes like fizzy water.");

                isAsking = false;
            }

            if (!inRange)
            {
                isAsking = false;

                if (DialogueManager.Instance != null)
                    DialogueManager.Instance.HideDialogue();
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
            anim.Update(0);
        }

        player.name = "CatPlayer";

        if (ladderObject != null)
            ladderObject.SetActive(true);

        Debug.Log("[PotionCabinet] Player transformed into CatPlayer.");
    }
}