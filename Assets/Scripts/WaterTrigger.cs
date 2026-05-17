using UnityEngine;

/// <summary>
/// Attach to a trigger collider sized to the water interior.
/// Calls WaterBody.Splash when a Rigidbody2D enters the water.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class WaterTrigger : MonoBehaviour
{
    public WaterBody waterBody;
    [Range(0f, 2f)] public float splashStrength = 1f;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (waterBody == null) return;
        if (!other.CompareTag("Player") && !other.CompareTag("Enemy")) return;
        waterBody.Splash(other.transform.position.x, splashStrength);
    }
}
