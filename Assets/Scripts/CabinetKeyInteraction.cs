using UnityEngine;

public class CabinetKeyInteraction : MonoBehaviour
{
    [Header("Key")]
    public string keyId = "LivingToHallKey";
    public Sprite keySprite;
    public GameObject keyVisual;

    [Header("Interaction")]
    public float interactionDistance = 2.0f;

    [Header("Sound")]
    public AudioSource pickupAudio;

    private Transform player;
    private bool isPlayerInRange;
    private bool hasKey = false;
    private bool waitingForJump = false;

    private void Update()
    {
        if (player == null)
        {
            FindPlayer();

            if (player == null)
                return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        isPlayerInRange = distance <= interactionDistance;

        if (!isPlayerInRange)
        {
            if (waitingForJump)
            {
                waitingForJump = false;

                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.HideSingleDialogue();
                }
            }

            return;
        }

        if (hasKey) return;

        bool isCat = player.name == "CatPlayer";

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isCat)
            {
                waitingForJump = true;

                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.ShowDialogue(
                        "I think I can reach it. I can jump! (Press J)",
                        false
                    );
                }
            }
            else
            {
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.ShowDialogue(
                        "I cannot reach the key."
                    );
                }
            }
        }

        if (waitingForJump && Input.GetKeyDown(KeyCode.J))
        {
            GrabKey();
        }
    }

    private void GrabKey()
    {
        if (InventoryManager.Instance == null) return;

        if (InventoryManager.Instance.IsFull())
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue(
                    "I cannot carry any more items."
                );
            }

            return;
        }

        bool added = InventoryManager.Instance.AddItem(keyId, keySprite);

        if (added)
        {
            hasKey = true;
            waitingForJump = false;

            if (pickupAudio != null)
            {
                pickupAudio.Play();
            }

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue("Key grabbed.");
            }

            if (keyVisual != null)
            {
                keyVisual.SetActive(false);
            }
        }
    }

    private void FindPlayer()
    {
        GameObject p = GameObject.Find("CatPlayer");

        if (p == null)
        {
            p = GameObject.Find("GoldenRetrieverPlayer");
        }

        if (p == null)
        {
            p = GameObject.FindWithTag("Player");
        }

        if (p != null)
        {
            player = p.transform;
        }
    }
}