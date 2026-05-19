using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Pulses a Light2D intensity with a sine wave (Scale Wave approach from the
/// Unity 2D Lighting for Pixel Art tutorial, applied to light intensity).
/// Attach to the RimLight2D child of dog_orb.
/// Enabled/disabled by DogOrbInteraction.SolveSequence together with CrystalGlowPulse.
/// </summary>
[RequireComponent(typeof(Light2D))]
public class OrbRimLightPulse : MonoBehaviour
{
    [Header("Intensity Wave")]
    public float intensityMin  = 6f;
    public float intensityMax  = 14f;
    public float pulseSpeed    = 1.1f;   // sine frequency
    public float pulseOffset   = 0f;     // phase offset (randomised in Awake)

    [Header("Radius Wave")]
    public float radiusMin     = 2.8f;
    public float radiusMax     = 4.2f;
    public float radiusSpeed   = 0.7f;   // slightly different frequency for organic feel

    private Light2D _light;

    void Awake()
    {
        _light      = GetComponent<Light2D>();
        pulseOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        float t = Time.time;

        // Intensity sine wave (Scale Wave on light output)
        float tI = (Mathf.Sin(t * pulseSpeed + pulseOffset) + 1f) * 0.5f;
        _light.intensity = Mathf.Lerp(intensityMin, intensityMax, tI);

        // Outer radius gentle breathe (secondary wave, offset phase)
        float tR = (Mathf.Sin(t * radiusSpeed + pulseOffset + 1.3f) + 1f) * 0.5f;
        _light.pointLightOuterRadius = Mathf.Lerp(radiusMin, radiusMax, tR);
    }
}
