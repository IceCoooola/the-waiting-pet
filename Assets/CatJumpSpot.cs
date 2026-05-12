using System.Collections;
using UnityEngine;

public class CatJumpSpot : MonoBehaviour
{
    [Header("Destination")]
    public Transform destinationPoint;

    [Header("Move To Bookshelf")]
    public float moveDuration = 0.15f;

    private bool isBeingUsed = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null)
            player.EnterCatJumpSpot(this);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerMovement player = other.GetComponent<PlayerMovement>();

        if (player != null)
            player.ExitCatJumpSpot(this);
    }

    public IEnumerator MovePlayerToBookshelf(Transform player, Rigidbody2D rb, PlayerMovement movementScript)
    {
        if (isBeingUsed) yield break;
        if (destinationPoint == null) yield break;

        isBeingUsed = true;
        movementScript.SetSpecialMoving(true);

        Vector3 start = player.position;
        Vector3 end = destinationPoint.position;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;

            Vector3 nextPos = Vector3.Lerp(start, end, t);

            rb.MovePosition(nextPos);

            yield return null;
        }

        rb.MovePosition(end);
        player.position = end;

        movementScript.SetSpecialMoving(false);
        isBeingUsed = false;
    }
}