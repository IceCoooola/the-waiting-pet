using UnityEngine;

/// <summary>Pulsing additive glow circle around an orb.</summary>
public class OrbGlowPulse : MonoBehaviour
{
    private GameObject glowObj;
    private SpriteRenderer glowSR;
    public Color glowColor = new Color(1f, 0.85f, 0.3f, 0.25f);
    public float pulseSpeed = 1.2f;

    void OnEnable()
    {
        if (glowObj == null)
        {
            glowObj = new GameObject("OrbGlow");
            glowObj.transform.SetParent(transform);
            glowObj.transform.localPosition = Vector3.zero;
            glowObj.transform.localScale    = Vector3.one * 3f;
            glowSR = glowObj.AddComponent<SpriteRenderer>();
            glowSR.sprite = Resources.GetBuiltinResource<Sprite>("UI/Skin/Knob.psd");
            var mat = new Material(Shader.Find("Sprites/Default"));
            mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
            glowSR.material     = mat;
            glowSR.sortingOrder = 15;
        }
        glowObj.SetActive(true);
    }

    void Update()
    {
        if (glowSR == null) return;
        float a = Mathf.Lerp(0.10f, 0.35f, (Mathf.Sin(Time.time * pulseSpeed) + 1f) * 0.5f);
        glowSR.color = new Color(glowColor.r, glowColor.g, glowColor.b, a);
    }

    void OnDisable() { if (glowObj != null) glowObj.SetActive(false); }
}
