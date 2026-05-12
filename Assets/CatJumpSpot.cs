using System.Collections;
using UnityEngine;

public class CatJumpSpot : MonoBehaviour
{
    [Header("Destination")]
    public Transform destinationPoint;

    [Header("Special Jump")]
    public float jumpDuration = 0.35f;

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

    public IEnumerator PerformSpecialJump(Transform player, Rigidbody2D rb, PlayerMovement movementScript)
    {
        if (isBeingUsed) yield break;
        if (destinationPoint == null) yield break;

        isBeingUsed = true;
        movementScript.SetSpecialJumping(true);

        Collider2D playerCollider = player.GetComponent<Collider2D>();

        if (playerCollider != null)
            playerCollider.enabled = false;

        Vector3 start = player.position;
        Vector3 end = destinationPoint.position;

        float elapsed = 0f;

        while (elapsed < jumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / jumpDuration;

            player.position = Vector3.Lerp(start, end, t);

            yield return null;
        }

        player.position = end;

        yield return new WaitForSeconds(0.05f);

        if (playerCollider != null)
            playerCollider.enabled = true;

        movementScript.SetSpecialJumping(false);
        isBeingUsed = false;
    }
}