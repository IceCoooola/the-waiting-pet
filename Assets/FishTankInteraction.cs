using UnityEngine;

public class FishTankInteraction : MonoBehaviour
{
    public enum TransitionDirection { None, Up, Down, Left, Right }

    [Header("Transition Settings")]
    public Transform destination;
    public GameObject roomToDisable;
    public GameObject roomToEnable;
    public TransitionDirection requiredDirection = TransitionDirection.None;
    public bool transformToFish = false;

    [Header("Required Potion")]
    public string requiredPotionId = "FishPotion";

    [Header("Dialogue")]
    [TextArea]
    public string noPotionDialogue = "Hmm... A fish tank in an attic?";

    [TextArea]
    public string wrongPotionDialogue = "The fish stares at it... judgmentally.";

    [TextArea]
    public string[] correctPotionDialoguePages;

    private bool isPlayerInRange = false;
    private GameObject player;

    private int currentDialoguePageIndex = -1;

    private static float lastTransitionTime = 0f;
    private const float transitionCooldown = 0.5f;

    private void Update()
    {
        if (!isPlayerInRange) return;

        if (currentDialoguePageIndex != -1)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AdvanceCorrectDialogue();
            }
            return;
        }

        if (IsPressingRequiredKey())
        {
            TryUseFishTank();
        }
    }

    private void TryUseFishTank()
    {
        if (InventoryManager.Instance == null) return;

        if (InventoryManager.Instance.HasItem(requiredPotionId))
        {
            InventoryManager.Instance.RemoveItem(requiredPotionId);

            if (correctPotionDialoguePages != null && correctPotionDialoguePages.Length > 0)
            {
                currentDialoguePageIndex = 0;
                PlayerMovement.movementLocked = true;

                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.ShowDialogue(correctPotionDialoguePages[0], false);
                }
            }
            else
            {
                PerformTransition();
            }
        }
        else if (PlayerHasAnyPotion())
        {
            ShowText(wrongPotionDialogue);
        }
        else
        {
            ShowText(noPotionDialogue);
        }
    }

    private void AdvanceCorrectDialogue()
    {
        currentDialoguePageIndex++;

        if (currentDialoguePageIndex < correctPotionDialoguePages.Length)
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue(correctPotionDialoguePages[currentDialoguePageIndex], false);
            }
        }
        else
        {
            currentDialoguePageIndex = -1;
            PlayerMovement.movementLocked = false;

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.HideDialogue();
            }

            PerformTransition();
        }
    }

    private bool PlayerHasAnyPotion()
    {
        string[] potionIds =
        {
            "HumanPotion",
            "BirdPotion",
            "DeerPotion",
            "DogPotion",
            "CatPotion",
            "BearPotion",
            "FishPotion"
        };

        foreach (string potionId in potionIds)
        {
            if (InventoryManager.Instance.HasItem(potionId))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsPressingRequiredKey()
    {
        switch (requiredDirection)
        {
            case TransitionDirection.Up:
                return Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow);

            case TransitionDirection.Down:
                return Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);

            case TransitionDirection.Left:
                return Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow);

            case TransitionDirection.Right:
                return Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow);

            case TransitionDirection.None:
                return Input.GetKeyDown(KeyCode.Space);

            default:
                return false;
        }
    }

    private void PerformTransition()
    {
        if (player == null) return;
        if (Time.time - lastTransitionTime < transitionCooldown) return;

        lastTransitionTime = Time.time;

        if (destination != null)
        {
            player.transform.position = destination.position;
        }

        if (transformToFish)
        {
            PlayerAppearanceSwitcher switcher = player.GetComponent<PlayerAppearanceSwitcher>();

            if (switcher != null)
            {
                switcher.SwitchToFish();
            }
        }

        if (roomToDisable != null)
        {
            roomToDisable.SetActive(false);
        }

        if (roomToEnable != null)
        {
            roomToEnable.SetActive(true);
        }
    }

    private void ShowText(string text)
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowDialogue(text, false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.name.Contains("Dog") || other.GetComponent<PlayerMovement>() != null)
        {
            isPlayerInRange = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.name.Contains("Dog") || other.GetComponent<PlayerMovement>() != null)
        {
            isPlayerInRange = false;

            if (currentDialoguePageIndex != -1)
            {
                currentDialoguePageIndex = -1;
                PlayerMovement.movementLocked = false;
            }

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.HideDialogue();
            }
        }
    }
}