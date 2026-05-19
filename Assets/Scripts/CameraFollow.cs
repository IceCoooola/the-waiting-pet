using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Tooltip("Drag your Dog GameObject here")]
    public Transform target;

    [Tooltip("How smoothly the camera catches up. Smaller = snappier, larger = floatier.")]
    public float smoothTime = 0.15f;

    [Tooltip("Camera Z must stay at -10 for 2D")]
    public float cameraZ = -10f;

    Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desired = new Vector3(target.position.x, target.position.y, cameraZ);
        transform.position = Vector3.SmoothDamp(transform.position, desired, ref velocity, smoothTime);
    }
}
