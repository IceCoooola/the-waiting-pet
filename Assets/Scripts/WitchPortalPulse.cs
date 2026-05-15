using UnityEngine;

/// <summary>Pulses the witch portal glow to simulate a bubbling cauldron underwater.</summary>
public class WitchPortalPulse : MonoBehaviour
{
    public float pulseSpeed = 1.2f;
    public float scaleMin = 0.85f;
    public float scaleMax = 1.15f;
    public float rotateSpeed = 18f;
    private Vector3 baseScale;

    void Start() => baseScale = transform.localScale;

    void Update()
    {
        float t = (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f;
        float s = Mathf.Lerp(scaleMin, scaleMax, t);
        transform.localScale = baseScale * s;
        transform.Rotate(0f, 0f, rotateSpeed * Time.deltaTime);
    }
}
