using UnityEngine;

/// <summary>
/// Gentle bobbing motion on Y-axis using Mathf.Sin.
/// Simulates floating items on water surface.
/// </summary>
public class FloatBob : MonoBehaviour
{
    [Header("Bobbing")]
    public float amplitude  = 0.08f;   // how far it moves up/down
    public float frequency  = 0.6f;    // how fast it bobs
    public float phaseOffset = 0f;     // offset so items dont all sync

    private Vector3 startPos;

    void Awake()
    {
        startPos     = transform.position;
        // Random phase so grouped items feel independent
        phaseOffset  = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        float y = startPos.y + Mathf.Sin(Time.time * frequency + phaseOffset) * amplitude;
        transform.position = new Vector3(startPos.x, y, startPos.z);
    }
}
