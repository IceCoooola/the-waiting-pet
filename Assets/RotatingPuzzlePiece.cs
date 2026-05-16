using UnityEngine;

public class RotatingPuzzlePiece : MonoBehaviour
{
    [Header("Correct Rotation Step")]
    [Tooltip("0 = default rotation, 1 = 90 degrees, 2 = 180 degrees, 3 = 270 degrees")]
    public int correctRotationStep = 0;

    private int currentRotationStep = 0;
    private Quaternion initialRotation;

    private void Start()
    {
        initialRotation = transform.rotation;
    }

    public void RotatePiece()
    {
        currentRotationStep++;

        if (currentRotationStep > 3)
        {
            currentRotationStep = 0;
        }

        float targetZ = currentRotationStep * 90f;

        transform.rotation =
            initialRotation *
            Quaternion.Euler(0f, 0f, targetZ);

        Debug.Log(
            gameObject.name +
            " rotation step: " +
            currentRotationStep
        );
    }

    public bool IsCorrect()
    {
        return currentRotationStep == correctRotationStep;
    }
}