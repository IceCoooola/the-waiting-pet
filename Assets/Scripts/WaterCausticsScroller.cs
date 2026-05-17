using UnityEngine;

/// <summary>
/// Animates water caustics by moving the sprite position slowly across
/// the interior, wrapping when it exits bounds. Uses two overlapping
/// instances moving in opposite directions for a realistic crossing effect.
/// Position scrolling works on SpriteRenderer (UV offset does not).
/// </summary>
public class WaterCausticsScroller : MonoBehaviour
{
    [Header("Movement")]
    public float speedX =  0.12f;
    public float speedY =  0.05f;

    [Header("Bounds (world space)")]
    public float boundsMinX = -60.83f;
    public float boundsMaxX = -45.59f;
    public float boundsMinY =   4.57f;
    public float boundsMaxY =  12.41f;

    [Header("Alpha Pulse")]
    public float pulseSpeed = 0.4f;
    public float alphaMin   = 0.10f;
    public float alphaMax   = 0.22f;

    private SpriteRenderer sr;
    private Vector3 startPos;
    private float width, height;

    void Start()
    {
        sr       = GetComponent<SpriteRenderer>();
        startPos = transform.position;
        width    = boundsMaxX - boundsMinX;
        height   = boundsMaxY - boundsMinY;
    }

    void Update()
    {
        float t = Time.time;

        // Move position, wrap when outside bounds
        float newX = startPos.x + (t * speedX) % width;
        float newY = startPos.y + (t * speedY) % height;

        // Keep within bounds using modulo wrap
        if (newX > boundsMaxX) newX -= width;
        if (newY > boundsMaxY) newY -= height;

        transform.position = new Vector3(newX, newY, transform.position.z);

        // Pulse alpha
        float alpha = Mathf.Lerp(alphaMin, alphaMax,
            (Mathf.Sin(t * pulseSpeed) + 1f) * 0.5f);
        var c = sr.color;
        sr.color = new Color(c.r, c.g, c.b, alpha);
    }
}
