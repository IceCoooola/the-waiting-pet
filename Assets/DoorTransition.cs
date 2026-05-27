using UnityEngine;

public class DoorTransition : MonoBehaviour
{
    public enum TransitionDirection { None, Up, Down, Left, Right, Any }

    [Header("Transition Settings")]
    public Transform destination;
    public GameObject roomToDisable;
    public GameObject roomToEnable;
    public TransitionDirection requiredDirection;
    public bool triggerOnTouch = false;
    public bool transformToFish = false;

    [Header("Unlock Dialogue")]
    public string[] unlockDialoguePages;
    private int currentUnlockPageIndex = -1;

    [Header("Lock Settings")]
    public string doorId; // Unique ID for this door
    public bool isLocked = false;
    public string requiredKeyId = "Room1Key";

    [TextArea]
    public string lockedDialogue = "The door is locked,\nwhere's the key?";

    private bool isPlayerInRange = false;
private GameObject player;

    private DoorSoundEffect soundEffect;

    private static float lastTransitionTime = 0f;
    private const float transitionCooldown = 0.5f;

    private void Start()
    {
        if (!string.IsNullOrEmpty(doorId) && GameData.IsDoorUnlocked(doorId))
        {
            isLocked = false;
        }
    }

    private void Awake()
{
        soundEffect = GetComponent<DoorSoundEffect>();
    }

    private void Update()
    {
        if (!isPlayerInRange) return;

        // Unlock dialogue pages
        if (currentUnlockPageIndex != -1)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AdvanceUnlockDialogue();
            }

            return;
        }

        // Immediate transition
        if (triggerOnTouch && !isLocked)
        {
            PerformTransition();
            return;
        }

        // Directional transition
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
        if (InventoryManager.Instance != null &&
            InventoryManager.Instance.HasItem(requiredKeyId))
        {
            isLocked = false;
            if (!string.IsNullOrEmpty(doorId))
            {
                GameData.UnlockDoor(doorId);
            }

            InventoryManager.Instance.RemoveItem(requiredKeyId);

            if (unlockDialoguePages != null &&
                unlockDialoguePages.Length > 0)
            {
                currentUnlockPageIndex = 0;

                DialogueManager.Instance.ShowDialogue(
                    unlockDialoguePages[0],
                    false,
                    0, null, false, true
                );

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
            // Locked sound
            soundEffect?.PlayLocked();

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
            DialogueManager.Instance.ShowDialogue(
                unlockDialoguePages[currentUnlockPageIndex],
                false,
                0, null, false, true
            );
        }
else
        {
            currentUnlockPageIndex = -1;

            PlayerMovement.movementLocked = false;

            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.HideDialogue();
            }

            PerformTransition();
        }
    }

    private bool IsPressingRequiredKey()
    {
        if (isLocked)
        {
            return Input.GetKeyDown(KeyCode.Space);
        }

        switch (requiredDirection)
        {
            case TransitionDirection.Up:
                return Input.GetKey(KeyCode.W) ||
                       Input.GetKey(KeyCode.UpArrow);

            case TransitionDirection.Down:
                return Input.GetKey(KeyCode.S) ||
                       Input.GetKey(KeyCode.DownArrow);

            case TransitionDirection.Left:
                return Input.GetKey(KeyCode.A) ||
                       Input.GetKey(KeyCode.LeftArrow);

            case TransitionDirection.Right:
                return Input.GetKey(KeyCode.D) ||
                       Input.GetKey(KeyCode.RightArrow);

            case TransitionDirection.None:
                return Input.GetKey(KeyCode.Space);

            default:
                return false;
        }
    }

    private void PerformTransition()
    {
        if (player == null) return;

        if (Time.time - lastTransitionTime < transitionCooldown)
            return;

        lastTransitionTime = Time.time;

        // Door open sound
        soundEffect?.PlayOpen();

        // Teleport player
        if (destination != null)
        {
            player.transform.position = destination.position;
        }

        // Fish transformation
        if (transformToFish)
        {
            PlayerAppearanceSwitcher switcher =
                player.GetComponent<PlayerAppearanceSwitcher>();

            if (switcher != null)
            {
                switcher.SwitchToFish();
            }
        }

        // Room switching
        if (roomToDisable != null)
        {
            roomToDisable.SetActive(false);
        }

        if (roomToEnable != null)
        {
            roomToEnable.SetActive(true);
        }

        Debug.Log(
            $"Transitioned using {requiredDirection} " +
            $"from {roomToDisable?.name} " +
            $"to {roomToEnable?.name}"
        );
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") ||
            other.name.Contains("Dog") ||
            other.GetComponent<PlayerMovement>() != null)
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
        if (other.CompareTag("Player") ||
            other.name.Contains("Dog") ||
            other.GetComponent<PlayerMovement>() != null)
        {
            isPlayerInRange = false;

            if (currentUnlockPageIndex != -1)
            {
                currentUnlockPageIndex = -1;

                PlayerMovement.movementLocked = false;

                if (DialogueManager.Instance != null)
                {
                    DialogueManager.Instance.HideDialogue();
                }
            }
            else
            {
                // Only hide single-page dialogues if the player walked away, 
                // but NOT if the room was just disabled (e.g. during a transition).
                // This prevents the "Door unlocked!" message from disappearing instantly.
                if (DialogueManager.Instance != null && gameObject.activeInHierarchy)
                {
                    DialogueManager.Instance.HideSingleDialogue();
                }
            }
}
    }
}