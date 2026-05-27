using UnityEngine;
using System.Collections.Generic;

public class LadderClimber : MonoBehaviour
{
    private List<Transform> ladderPoints = new List<Transform>();
    private int currentPointIndex = -1;
    private bool isOnLadder = false;
    private Rigidbody2D rb;
    private float originalGravity;
    private GameObject currentLadder;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        // Reset state if current ladder becomes inactive (due to room transition)
        if (isOnLadder && currentLadder != null && !currentLadder.activeInHierarchy)
        {
            isOnLadder = false;
        }

        if (isOnLadder && Input.GetKeyDown(KeyCode.J))
        {
            // Silence if player is a fish
            PlayerAppearanceSwitcher switcher = GetComponent<PlayerAppearanceSwitcher>();
            if (switcher != null && switcher.CurrentForm == "Fish")
            {
                return;
            }

            // Silence dialogue and ladder action in restricted rooms
            bool inRestrictedRoom = false;
            GameObject[] allRooms = GameObject.FindGameObjectsWithTag("Untagged");
            foreach (var room in allRooms)
            {
                if (room.activeInHierarchy && (room.name.Contains("2nd Floor") || room.name.Contains("Basement")))
                {
                    inRestrictedRoom = true;
                    break;
                }
            }

            if (inRestrictedRoom)
            {
                return;
            }

            ClimbToNextPoint();
        }
    }

    void ClimbToNextPoint()
    {
        if (ladderPoints.Count == 0) return;

        // Check if we are already at the top of the ladder
        if (currentPointIndex >= ladderPoints.Count - 1)
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.ShowDialogue("I don't want to jump back. It looks so high!");
            }
            return;
        }

        currentPointIndex++;

        if (currentPointIndex < ladderPoints.Count)
        {
            StartCoroutine(JumpToPoint(ladderPoints[currentPointIndex].position));
        }
    }

    private System.Collections.IEnumerator JumpToPoint(Vector3 targetPos)
    {
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null) movement.SetSpecialMoving(true);

        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;

        // Teleport slightly above the point to clear colliders
        Vector3 finalPos = targetPos + new Vector3(0, 0.2f, 0);

        transform.position = finalPos;
        rb.position = finalPos;

        // Wait for physics update
        yield return new WaitForFixedUpdate();

        if (movement != null) movement.SetSpecialMoving(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Specific Step Detection (Most precise)
        LadderStep step = collision.GetComponent<LadderStep>();
        if (step != null)
        {
            isOnLadder = true;
            currentPointIndex = step.stepIndex;
            return;
        }

        // 2. Main Ladder Zone Detection
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = true;
            currentLadder = collision.gameObject;
            ladderPoints.Clear();
            foreach (Transform child in collision.transform)
            {
                ladderPoints.Add(child);
            }
            
            // Start at -1 if below the first point, so first jump hits Point 1
            if (ladderPoints.Count > 0 && transform.position.y < ladderPoints[0].position.y - 0.5f)
            {
                currentPointIndex = -1;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder") && currentLadder == collision.gameObject)
        {
            isOnLadder = false;
            rb.gravityScale = originalGravity;
        }
    }

    public bool IsClimbing => isOnLadder;
}