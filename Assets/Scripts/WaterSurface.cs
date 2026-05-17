using UnityEngine;

/// <summary>
/// Spring-based water surface line with organic ambient wobble.
/// The surface 'breathes' independently at each node using perlin noise,
/// so no two points move at the same rate — eliminating the uniform sheet feel.
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class WaterSurface : MonoBehaviour
{
    [Header("Spring Simulation")]
    public int   nodeCount  = 48;
    public float springK    = 60f;
    public float damping    = 5f;
    public float spread     = 0.06f;

    [Header("Ambient Wobble (replaces flat edge)")]
    public float wobbleAmp  = 0.018f;   // max 2px displacement at standard zoom
    public float wobbleFreq = 1.8f;     // spatial frequency along surface
    public float wobbleSpeed = 0.09f;   // SLOW — almost imperceptible drift
    public float wobbleFreq2 = 3.1f;    // second layer, different freq
    public float wobbleSpeed2 = 0.04f;

    [Header("Visual")]
    public float lineWidth    = 0.09f;
    public Color surfaceColor = new Color(0.55f, 0.82f, 0.95f, 0.85f);

    private float[] heights;
    private float[] velocities;
    private float   waterWidth;
    private float   waterHeight;
    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        lr.useWorldSpace     = false;
        lr.startWidth        = lr.endWidth = lineWidth;
        lr.startColor        = lr.endColor = surfaceColor;
        lr.positionCount     = nodeCount;

        var wb       = GetComponentInParent<WaterBody>();
        waterWidth   = wb != null ? wb.width  : 15.63f;
        waterHeight  = wb != null ? wb.height :  8.03f;
        heights      = new float[nodeCount];
        velocities   = new float[nodeCount];
    }

    void FixedUpdate()
    {
        float t    = Time.fixedTime;
        float step = waterWidth / (nodeCount - 1);

        // Spring forces
        for (int i = 0; i < nodeCount; i++) {
            float force    = -springK * heights[i] - damping * velocities[i];
            velocities[i] += force * Time.fixedDeltaTime;
            heights[i]    += velocities[i] * Time.fixedDeltaTime;
        }

        // Neighbour propagation
        for (int i = 0; i < nodeCount - 1; i++) {
            float d = spread * (heights[i+1] - heights[i]);
            velocities[i+1] -= d;
            velocities[i]   += d;
        }

        // Update line positions with ambient wobble layered on top
        for (int i = 0; i < nodeCount; i++) {
            float x = i * step;
            // Two slow sine waves at different frequencies — breaks uniformity
            float w1 = Mathf.Sin(x * wobbleFreq   + t * wobbleSpeed)   * wobbleAmp;
            float w2 = Mathf.Sin(x * wobbleFreq2  + t * wobbleSpeed2   + 1.3f) * wobbleAmp * 0.45f;
            // Spring offset + ambient wobble
            float y  = waterHeight + heights[i] + w1 + w2;
            lr.SetPosition(i, new Vector3(x, y, -0.05f));
        }
    }

    public void AddSplash(float uvX, float strength = 1f)
    {
        int node = Mathf.RoundToInt(uvX * (nodeCount - 1));
        node = Mathf.Clamp(node, 0, nodeCount - 1);
        velocities[node] -= 2.2f * strength;
    }
}
