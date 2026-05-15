using UnityEngine;

/// <summary>
/// Pans a semi-transparent caustics texture across the basement floor
/// to simulate underwater light patterns.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class BasementCaustics : MonoBehaviour
{
    [Header("Pan Speed")]
    public float speedX = 0.04f;
    public float speedY = 0.02f;

    [Header("Pulse")]
    public float pulseSpeed = 0.8f;
    public float pulseAmplitude = 0.06f;
    public float baseAlpha = 0.18f;

    private SpriteRenderer sr;
    private Vector3 startPos;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        startPos = transform.position;
    }

    void Update()
    {
        // Pan the caustics sprite slowly across the floor
        transform.position = startPos + new Vector3(
            Mathf.Sin(Time.time * speedX * 0.5f) * 2f,
            speedY * Time.time % 3f,
            0f
        );

        // Pulse alpha to simulate light ripple
        float alpha = baseAlpha + Mathf.Sin(Time.time * pulseSpeed) * pulseAmplitude;
        var c = sr.color;
        sr.color = new Color(c.r, c.g, c.b, Mathf.Clamp01(alpha));
    }
}
