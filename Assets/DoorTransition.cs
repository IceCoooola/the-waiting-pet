using UnityEngine;

public class DoorTransition : MonoBehaviour
{
    public enum TransitionDirection { None, Up, Down, Left, Right }

    [Header("Transition Settings")]
    public Transform destination;      // Where the player teleports to
    public GameObject roomToDisable;   // The room we are leaving
    public GameObject roomToEnable;    // The room we are entering
    public TransitionDirection requiredDirection; // Key to press (W=Up, S=Down, etc.)

    [Header("Lock Settings")]
    public bool isLocked = false;
    public string requiredKeyId = "Room1Key";
    public string lockedDialogue = "the door is locked.";

    private bool isPlayerInRange = false;
    private GameObject player;

    private void Update()
    {
        // Use Input.GetKey instead of GetKeyDown so it works while holding the key
        if (isPlayerInRange && IsPressingRequiredKey())
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
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue("Door unlocked!");
            }
            PerformTransition();
        }
        else
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue(lockedDialogue);
            }
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

    private void PerformTransition()
    {
        if (player == null) return;

        // 1. Teleport player
        if (destination != null)
        {
            player.transform.position = destination.position;
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
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.name.Contains("Dog") || other.GetComponent<PlayerMovement>() != null)
        {
            isPlayerInRange = false;
        }
    }
}
