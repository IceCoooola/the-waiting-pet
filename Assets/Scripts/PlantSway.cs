using UnityEngine;

/// <summary>
/// Gentle underwater sway for plant sprites.
/// Rotates and slightly scales the object using Mathf.Sin,
/// simulating current movement without translating position.
/// </summary>
public class PlantSway : MonoBehaviour
{
    [Header("Sway")]
    public float swayAngle    = 4.5f;   // max rotation degrees
    public float swaySpeed    = 0.55f;  // sway frequency
    public float scaleAmount  = 0.025f; // subtle scale pulse
    public float phaseOffset  = 0f;

    private Vector3 baseScale;

    void Awake()
    {
        baseScale   = transform.localScale;
        phaseOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        float t = Time.time * swaySpeed + phaseOffset;

        // Rotation: slow side-to-side sway
        float angle = Mathf.Sin(t) * swayAngle;
        transform.localRotation = UnityEngine.Quaternion.Euler(0f, 0f, angle);

        // Subtle scale pulse (breathe)
        float s = 1f + Mathf.Sin(t * 1.3f) * scaleAmount;
        transform.localScale = new Vector3(baseScale.x * s, baseScale.y * s, 1f);
    }
}
