using UnityEngine;

/// <summary>
/// Drives the PixelWater shader on a quad mesh sized to the water interior.
/// Handles spring-based ripple triggers and exposes splash spawn point.
/// </summary>
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class WaterBody : MonoBehaviour
{
    [Header("Dimensions")]
    public float width  = 15.63f;
    public float height =  8.03f;

    [Header("Ripple")]
    [Range(0f, 1f)] public float rippleAmplitude = 0.8f;
    public float rippleDuration = 2.5f;

    [Header("References")]
    public ParticleSystem splashParticles;
    public WaterSurface   surface;

    private Material mat;
    private float    rippleStartTime = -99f;

    void Awake()
    {
        BuildQuad();
        mat = GetComponent<MeshRenderer>().material;
        mat.SetFloat("_RippleTime",  -99f);
        mat.SetFloat("_RippleAmp",     0f);
    }

    void BuildQuad()
    {
        var mf   = GetComponent<MeshFilter>();
        var mesh = new Mesh { name = "WaterQuad" };

        float w = width, h = height;
        mesh.vertices  = new Vector3[] {
            new Vector3(0, 0, 0), new Vector3(w, 0, 0),
            new Vector3(0, h, 0), new Vector3(w, h, 0)
        };
        mesh.uv = new Vector2[] {
            new Vector2(0,0), new Vector2(1,0),
            new Vector2(0,1), new Vector2(1,1)
        };
        mesh.triangles = new int[] { 0,2,1, 2,3,1 };
        mesh.RecalculateNormals();
        mf.mesh = mesh;
    }

    /// <summary>Call when something enters the water at a world-space X position.</summary>
    public void Splash(float worldX, float splashStrength = 1f)
    {
        // Convert world X to UV X
        float uvX = (worldX - transform.position.x) / width;
        uvX = Mathf.Clamp01(uvX);

        rippleStartTime = Time.time;
        mat.SetFloat( "_RippleTime",   rippleStartTime);
        mat.SetFloat( "_RippleAmp",    rippleAmplitude * splashStrength);
        mat.SetVector("_RippleCenter", new Vector4(uvX, 0.9f, 0, 0));

        if (splashParticles != null) {
            splashParticles.transform.position = new Vector3(worldX,
                transform.position.y + height, -0.1f);
            splashParticles.Emit(Mathf.RoundToInt(12 * splashStrength));
        }

        if (surface != null) surface.AddSplash(uvX, splashStrength);
    }

    void Update()
    {
        if (rippleStartTime > -10f && Time.time - rippleStartTime > rippleDuration) {
            rippleStartTime = -99f;
            mat.SetFloat("_RippleTime", -99f);
            mat.SetFloat("_RippleAmp",    0f);
        }
    }
}
