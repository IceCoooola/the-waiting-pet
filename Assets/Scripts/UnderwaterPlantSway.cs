using UnityEngine;

/// <summary>
/// Realistic underwater plant sway using two sine waves.
/// Top of plant sways most, base stays anchored.
/// Creates the look of plants moved by slow underwater currents.
/// </summary>
public class UnderwaterPlantSway : MonoBehaviour
{
    [Header("Sway")]
    public float swayAngle   = 8f;    // max degrees of rotation
    public float swaySpeed   = 0.5f;  // primary sway frequency
    public float swaySpeed2  = 0.31f; // secondary wave (irrational ratio = organic)
    public float swayWeight  = 0.65f; // blend between two waves

    [Header("Scale Breathe")]
    public float breatheAmount = 0.03f;
    public float breatheSpeed  = 0.42f;

    private float phase;
    private Vector3 baseScale;
    private Transform tf;

    void Awake()
    {
        tf        = transform;
        baseScale = tf.localScale;
        phase     = Random.Range(0f, Mathf.PI * 2f); // desync multiple plants
    }

    void Update()
    {
        float t = Time.time;

        // Two sine waves at different speeds multiplied = complex organic motion
        float wave1 = Mathf.Sin(t * swaySpeed  + phase);
        float wave2 = Mathf.Sin(t * swaySpeed2 + phase * 1.3f);
        float sway  = Mathf.Lerp(wave1, wave2, swayWeight) * swayAngle;

        tf.localRotation = Quaternion.Euler(0f, 0f, sway);

        // Subtle scale breathe (plant fronds expanding/contracting)
        float breathe = 1f + Mathf.Sin(t * breatheSpeed + phase) * breatheAmount;
        tf.localScale  = new Vector3(baseScale.x, baseScale.y * breathe, 1f);
    }
}
