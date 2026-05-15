using UnityEngine;

/// <summary>
/// Applies a full-screen murky teal tint to simulate being underwater.
/// Attach to Main Camera. Works on Built-in pipeline.
/// </summary>
[RequireComponent(typeof(Camera))]
public class BasementWaterTint : MonoBehaviour
{
    [Range(0f, 1f)] public float tintStrength = 0.28f;
    public Color waterColor = new Color(0.05f, 0.22f, 0.30f, 1f);
    public float rippleSpeed = 0.4f;
    public float rippleAmount = 0.012f;

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        // Simple tint blit using Graphics.Blit with a tinted material
        // Using a temp RT for color grading
        var temp = RenderTexture.GetTemporary(src.descriptor);
        Graphics.Blit(src, temp);

        // Draw teal overlay using GL
        RenderTexture.active = dst;
        Graphics.Blit(temp, dst);

        GL.PushMatrix();
        GL.LoadOrtho();
        GL.Begin(GL.QUADS);
        GL.Color(new Color(waterColor.r, waterColor.g, waterColor.b,
            tintStrength + Mathf.Sin(Time.time * rippleSpeed) * rippleAmount));
        GL.Vertex3(0, 0, 0);
        GL.Vertex3(0, 1, 0);
        GL.Vertex3(1, 1, 0);
        GL.Vertex3(1, 0, 0);
        GL.End();
        GL.PopMatrix();

        RenderTexture.ReleaseTemporary(temp);
    }
}
