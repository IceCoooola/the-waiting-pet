using UnityEngine;

public class RotatingPuzzlePiece : MonoBehaviour
{
    public float correctZRotation = 0f;
    public float rotationStep = 90f;

    private bool playerInRange = false;

    private void Update()
    {
        if (!playerInRange) return;

        if (Input.GetKeyDown(KeyCode.T))
        {
            RotatePiece();
        }
    }

    private void RotatePiece()
    {
        transform.Rotate(0f, 0f, rotationStep);

        float currentZ = NormalizeAngle(transform.eulerAngles.z);
        float correctZ = NormalizeAngle(correctZRotation);

        Debug.Log(gameObject.name + " rotation: " + currentZ);

        FootstepPuzzleManager manager = FindObjectOfType<FootstepPuzzleManager>();
        if (manager != null)
        {
            manager.CheckPuzzle();
        }
    }

    public bool IsCorrect()
    {
        float currentZ = NormalizeAngle(transform.eulerAngles.z);
        float correctZ = NormalizeAngle(correctZRotation);

        return Mathf.Abs(Mathf.DeltaAngle(currentZ, correctZ)) < 1f;
    }

    private float NormalizeAngle(float angle)
    {
        angle %= 360f;
        if (angle < 0) angle += 360f;
        return angle;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}