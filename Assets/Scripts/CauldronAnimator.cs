using UnityEngine;

/// <summary>
/// Animates the cauldron liquid: cycles through bubble/spew sprite frames
/// and pulses the green glow intensity. Runs entirely on a child SpriteRenderer.
/// </summary>
public class CauldronAnimator : MonoBehaviour
{
    [Header("Sprites")]
    public Sprite[] bubbleFrames;       // 4 frames from CauldronBubbles spritesheet
    public SpriteRenderer bubbleRenderer;

    [Header("Timing")]
    public float frameRate   = 6f;      // frames per second
    public float glowSpeed   = 1.8f;    // pulse speed
    public float glowMin     = 0.65f;
    public float glowMax     = 1.00f;

    [Header("Liquid Color Pulse")]
    public SpriteRenderer liquidRenderer; // child renderer showing cauldron body
    public Color liquidColorA = new Color(0.05f, 0.55f, 0.10f, 1f);
    public Color liquidColorB = new Color(0.45f, 0.95f, 0.30f, 1f);

    private float frameTimer;
    private int   currentFrame;

    void Update()
    {
        // Advance bubble animation frame
        frameTimer += Time.deltaTime;
        if (frameTimer >= 1f / frameRate) {
            frameTimer = 0f;
            currentFrame = (currentFrame + 1) % bubbleFrames.Length;
            if (bubbleRenderer != null && bubbleFrames.Length > 0)
                bubbleRenderer.sprite = bubbleFrames[currentFrame];
        }

        // Pulse alpha on bubble renderer for glow effect
        if (bubbleRenderer != null) {
            float t = (Mathf.Sin(Time.time * glowSpeed) + 1f) * 0.5f;
            var c   = bubbleRenderer.color;
            c.a     = Mathf.Lerp(glowMin, glowMax, t);
            bubbleRenderer.color = c;
        }

        // Subtle color pulse on the liquid layer
        if (liquidRenderer != null) {
            float t = (Mathf.Sin(Time.time * glowSpeed * 0.7f) + 1f) * 0.5f;
            liquidRenderer.color = Color.Lerp(liquidColorA, liquidColorB, t * 0.4f);
        }
    }
}
