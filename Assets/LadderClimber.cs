using UnityEngine;
using System.Collections.Generic;

public class LadderClimber : MonoBehaviour
{
    private List<Transform> ladderPoints = new List<Transform>();
    private int currentPointIndex = -1;
    private bool isOnLadder = false;
    private Rigidbody2D rb;
    private float originalGravity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravity = rb.gravityScale;
    }

    void Update()
    {
        if (isOnLadder && Input.GetKeyDown(KeyCode.J))
        {
            ClimbToNextPoint();
        }
    }

    void ClimbToNextPoint()
    {
        if (ladderPoints.Count == 0) return;

        rb.gravityScale = 0;
        rb.linearVelocity = Vector2.zero;

        currentPointIndex++;

        if (currentPointIndex < ladderPoints.Count)
        {
            transform.position = ladderPoints[currentPointIndex].position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If it touches the ladder tagged object, it brings point info
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = true;
            ladderPoints.Clear();
            foreach (Transform child in collision.transform)
            {
                ladderPoints.Add(child);
            }
            currentPointIndex = -1;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ladder"))
        {
            isOnLadder = false;
            rb.gravityScale = originalGravity;
            ladderPoints.Clear();
        }
    }

    public bool IsClimbing => isOnLadder;
}