using UnityEngine;

/// <summary>
/// Draws a radial vignette on the camera using OnRenderImage.
/// Simulates the claustrophobic darkness of a flooded basement.
/// Works on Built-in Render Pipeline without Post-Processing package.
/// </summary>
[RequireComponent(typeof(Camera))]
public class BasementVignette : MonoBehaviour
{
    [Range(0f, 1f)] public float intensity = 0.55f;
    [Range(0.1f, 5f)] public float smoothness = 2.2f;
    public Color vignetteColor = new Color(0f, 0.04f, 0.08f, 1f);

    private Material mat;

    void OnEnable()
    {
        var shader = Shader.Find("Hidden/BasementVignette");
        if (shader != null)
            mat = new Material(shader);
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (mat == null) { Graphics.Blit(src, dst); return; }
        mat.SetFloat("_Intensity", intensity);
        mat.SetFloat("_Smoothness", smoothness);
        mat.SetColor("_Color", vignetteColor);
        Graphics.Blit(src, dst, mat);
    }

    void OnDisable() { if (mat) DestroyImmediate(mat); }
}
