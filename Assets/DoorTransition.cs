using UnityEngine;

public class DoorTransition : MonoBehaviour
{
    public enum TransitionDirection { None, Up, Down, Left, Right, Any }

    [Header("Transition Settings")]
    public Transform destination;      // Where the player teleports to
    public GameObject roomToDisable;   // The room we are leaving
    public GameObject roomToEnable;    // The room we are entering
    public TransitionDirection requiredDirection; // Key to press (W=Up, S=Down, etc.)
    public bool triggerOnTouch = false; // If true, transit immediately on touch
    public bool transformToFish = false; // If true, player turns into a fish

    [Header("Unlock Dialogue")]
    public string[] unlockDialoguePages; // If set, these pages show on unlock instead of "Door unlocked!"
    private int currentUnlockPageIndex = -1;

    [Header("Lock Settings")]
public bool isLocked = false;
    public string requiredKeyId = "Room1Key";
    public string lockedDialogue = "The door is locked,\nwhere's the key?";

    private bool isPlayerInRange = false;
    private GameObject player;

    private void Update()
    {
        if (!isPlayerInRange) return;

        // Handle multi-page unlock dialogue
        if (currentUnlockPageIndex != -1)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AdvanceUnlockDialogue();
            }
            return;
        }

        // If triggerOnTouch is enabled and not locked, transit immediately
        if (triggerOnTouch && !isLocked)
        {
            PerformTransition();
            return;
        }

        // Use Input.GetKey instead of GetKeyDown so it works while holding the key
        if (IsPressingRequiredKey())
        {
            if (isLocked)
            {
                TryUnlock();
            }
            else
            {
                PerformTransition();
            }
        }
    }

    private void TryUnlock()
    {
        if (InventoryManager.Instance != null && InventoryManager.Instance.HasItem(requiredKeyId))
        {
            isLocked = false;
            InventoryManager.Instance.RemoveItem(requiredKeyId);
            
            if (unlockDialoguePages != null && unlockDialoguePages.Length > 0)
            {
                currentUnlockPageIndex = 0;
                DialogueManager.Instance.ShowDialogue(unlockDialoguePages[0], false);
                PlayerMovement.movementLocked = true;
            }
            else
            {
                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.ShowDialogue("Door unlocked!");
                }
                PerformTransition();
            }
        }
        else
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue(lockedDialogue);
            }
        }
    }

    private void AdvanceUnlockDialogue()
    {
        currentUnlockPageIndex++;
        if (currentUnlockPageIndex < unlockDialoguePages.Length)
        {
            DialogueManager.Instance.ShowDialogue(unlockDialoguePages[currentUnlockPageIndex], false);
        }
        else
        {
            currentUnlockPageIndex = -1;
            PlayerMovement.movementLocked = false;
            DialogueManager.Instance.HideDialogue();
            PerformTransition();
        }
    }

    private bool IsPressingRequiredKey()
    {
        // Only allow Space for interaction if locked, to prevent accidental triggers while walking into it
        if (isLocked)
        {
            return Input.GetKeyDown(KeyCode.Space);
        }
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
                return Input.GetKey(KeyCode.Space) || Input.GetKeyDown(KeyCode.Space);
            default:
                return false;
        }
    }

    private static float lastTransitionTime = 0f;
    private const float transitionCooldown = 0.5f;

    private void PerformTransition()
    {
        GetComponent<SoundEffect>()?.PlaySound();
        
        if (player == null) return;
        if (Time.time - lastTransitionTime < transitionCooldown) return;

        lastTransitionTime = Time.time;

        // 1. Teleport player
        if (destination != null)
        {
            player.transform.position = destination.position;
        }

        // 1.5. Transform to fish if requested
        if (transformToFish)
        {
            PlayerAppearanceSwitcher switcher = player.GetComponent<PlayerAppearanceSwitcher>();
            if (switcher != null)
            {
                switcher.SwitchToFish();
            }
        }

        // 2. Toggle rooms
// By disabling the room parent, this script stops running immediately, 
        // preventing multiple transitions in one frame.
        if (roomToDisable != null) roomToDisable.SetActive(false);
        if (roomToEnable != null) roomToEnable.SetActive(true);

        Debug.Log($"Transitioned using {requiredDirection} from {roomToDisable?.name} to {roomToEnable?.name}");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.name.Contains("Dog") || other.GetComponent<PlayerMovement>() != null)
        {
            isPlayerInRange = true;
            player = other.gameObject;

            if (triggerOnTouch && !isLocked)
            {
                PerformTransition();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.name.Contains("Dog") || other.GetComponent<PlayerMovement>() != null)
        {
            isPlayerInRange = false;
            if (currentUnlockPageIndex != -1)
            {
                currentUnlockPageIndex = -1;
                PlayerMovement.movementLocked = false;
                if (DialogueManager.Instance != null) DialogueManager.Instance.HideDialogue();
            }
        }
    }
}
