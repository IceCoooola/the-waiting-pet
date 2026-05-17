using UnityEngine;

/// <summary>
/// Animates the basement water shimmer overlay using overlapping sine waves.
/// Alpha breathes, color shifts between teal and deep blue, and a subtle
/// scale ripple simulates surface movement.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class WaterShimmerAnimator : MonoBehaviour
{
    [Header("Alpha Pulse")]
    [Range(0f,1f)] public float alphaMin  = 0.35f;
    [Range(0f,1f)] public float alphaMax  = 0.65f;
    public float alphaSpeed = 0.5f;

    [Header("Color Shift")]
    public Color colorA = new Color(0.05f, 0.20f, 0.42f, 1f);
    public Color colorB = new Color(0.04f, 0.35f, 0.40f, 1f);
    public float colorSpeed = 0.28f;

    [Header("Scale Ripple")]
    [Range(0f, 0.03f)] public float rippleAmount = 0.018f;
    public float rippleSpeedX = 0.7f;
    public float rippleSpeedY = 1.05f;

    private SpriteRenderer sr;
    private Vector3 baseScale;
    private float ao, co, rx, ry;

    void Awake()
    {
        sr        = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale;
        ao = Random.Range(0f, Mathf.PI * 2f);
        co = Random.Range(0f, Mathf.PI * 2f);
        rx = Random.Range(0f, Mathf.PI * 2f);
        ry = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        float t = Time.time;

        float alpha  = Mathf.Lerp(alphaMin, alphaMax,
            (Mathf.Sin(t * alphaSpeed + ao) + 1f) * 0.5f);

        float colorT = (Mathf.Sin(t * colorSpeed + co) + 1f) * 0.5f;
        Color c      = Color.Lerp(colorA, colorB, colorT);
        c.a          = alpha;
        sr.color     = c;

        float sx = 1f + Mathf.Sin(t * rippleSpeedX + rx) * rippleAmount;
        float sy = 1f + Mathf.Sin(t * rippleSpeedY + ry) * rippleAmount;
        transform.localScale = new Vector3(baseScale.x * sx, baseScale.y * sy, 1f);
    }
}
