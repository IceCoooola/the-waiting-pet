using UnityEngine;

/// <summary>
/// Crystal glow: ambient filled-circle breathe + radiating annulus pulse rings.
/// Uses URP Sprite-Unlit-Default with SrcAlpha/One additive blending.
/// Glow layers parented to orb's parent to bypass inherited scale.
/// </summary>
public class CrystalGlowPulse : MonoBehaviour
{
    public Color glowColor
    {
        get => crystalColorA;
        set {
            crystalColorA  = value;
            crystalColorB  = Color.Lerp(value, Color.white, 0.35f);
            innerGlowColor = new Color(value.r, value.g, value.b, 0.6f);
            pulseRingColor = value;
            outerGlowColor = new Color(value.r * 0.4f, value.g * 0.5f, value.b * 0.9f, 0.3f);
        }
    }
    public float pulseSpeed { get => crystalPulseSpeed; set => crystalPulseSpeed = value; }

    [Header("Crystal")]
    public Color crystalColorA     = new Color(0f, 0.616f, 1f, 1f);
    public Color crystalColorB     = new Color(0.6f, 0.9f,  1f, 1f);
    public float crystalPulseSpeed = 1.4f;

    [Header("Ambient Glow (filled circle)")]
    public Color innerGlowColor = new Color(0f, 0.616f, 1f, 0.6f);
    public float innerGlowSpeed = 0.9f;
    public float innerWorldMin  = 0.3f;
    public float innerWorldMax  = 0.6f;

    [Header("Pulse Rings (expanding annulus)")]
    public Color pulseRingColor = new Color(0f, 0.616f, 1f, 1f);
    public int   ringCount      = 3;
    public float ringPeriod     = 2.2f;
    public float ringWorldStart = 0.2f;
    public float ringWorldEnd   = 3.5f;
    public float ringAlphaMax   = 0.85f;
    public float ringAlphaCurve = 1.6f;

    [Header("Outer Halo")]
    public Color outerGlowColor = new Color(0f, 0.3f, 0.8f, 0.3f);
    public float outerGlowSpeed = 0.45f;
    public float outerWorldMin  = 0.55f;
    public float outerWorldMax  = 1.2f;

    [Header("Flicker")]
    public float flickerSpeed  = 7f;
    public float flickerAmount = 0.05f;

    private SpriteRenderer   crystalSR;
    private SpriteRenderer   innerSR, outerSR;
    private SpriteRenderer[] ringSRs;
    private float phaseA, phaseB, phaseC;
    private Material         additiveMat;

    // Two sprite shapes: soft filled circle (ambient) + sharp annulus (rings)
    private static Sprite _circleSprite;
    private static Sprite _ringSprite;

    void Awake()
    {
        crystalSR = GetComponent<SpriteRenderer>();
        phaseA = Random.Range(0f, Mathf.PI * 2f);
        phaseB = Random.Range(0f, Mathf.PI * 2f);
        phaseC = Random.Range(0f, Mathf.PI * 2f);

        if (_circleSprite == null) _circleSprite = MakeCircle(128, false);
        if (_ringSprite   == null) _ringSprite   = MakeCircle(128, true);

        BuildAdditiveMaterial();
        BuildLayers();
    }

    // isRing=false -> soft filled radial gradient
    // isRing=true  -> annulus: dark center, bright band, dark edge
    static Sprite MakeCircle(int res, bool isRing)
    {
        var tex = new Texture2D(res, res, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode   = TextureWrapMode.Clamp;
        float c = res * 0.5f;
        for (int y = 0; y < res; y++)
        for (int x = 0; x < res; x++) {
            float dx   = (x + 0.5f) - c;
            float dy   = (y + 0.5f) - c;
            float dist = Mathf.Sqrt(dx * dx + dy * dy) / c; // 0=center 1=edge
            float a;
            if (!isRing) {
                // Filled gradient: bright center, fade to edge
                a = Mathf.Clamp01(1f - Mathf.Pow(dist, 1.2f));
            } else {
                // Annulus: peak brightness at dist~0.65, fade inside and outside
                float inner = Mathf.SmoothStep(0.25f, 0.55f, dist);
                float outer = 1f - Mathf.SmoothStep(0.65f, 0.98f, dist);
                a = inner * outer;
            }
            tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
        }
        tex.Apply();
        return Sprite.Create(tex, new Rect(0,0,res,res), new Vector2(0.5f,0.5f), res);
    }

    void BuildAdditiveMaterial()
    {
        // URP Sprite-Unlit-Default with SrcAlpha/One = additive blending
        var shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default")
                  ?? Shader.Find("Sprites/Default");
        additiveMat = new Material(shader);
        additiveMat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        additiveMat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.One);
        additiveMat.SetInt("_ZWrite", 0);
        additiveMat.renderQueue = 3001;
    }

    // 1 world-unit sprite (128px at 128 PPU). R2S: world radius -> local scale
    // Sprite half-width = 0.5, so localScale = worldRadius / 0.5 = worldRadius * 2
    float R2S(float r) => r * 2f;

    void BuildLayers()
    {
        int   baseOrder = crystalSR.sortingOrder;
        string layerName = crystalSR.sortingLayerName;
        var   parent    = transform.parent ?? transform;

        outerSR = MakeLayer("OrbOuterHalo",  _circleSprite, outerGlowColor, baseOrder + 1, parent, layerName);
        ringSRs = new SpriteRenderer[ringCount];
        for (int i = 0; i < ringCount; i++)
            ringSRs[i] = MakeLayer("OrbPulseRing_"+i, _ringSprite, Color.clear, baseOrder + 2, parent, layerName);
        innerSR = MakeLayer("OrbInnerGlow",  _circleSprite, innerGlowColor, baseOrder + 3, parent, layerName);
    }

    SpriteRenderer MakeLayer(string n, Sprite sp, Color c, int order, Transform par, string layerName)
    {
        var go = new GameObject(n);
        go.transform.SetParent(par);
        go.transform.position   = transform.position;
        go.transform.localScale = Vector3.one;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite           = sp;
        sr.material         = new Material(additiveMat);
        sr.color            = c;
        sr.sortingOrder     = order;
        sr.sortingLayerName = layerName;
        return sr;
    }

    void Update()
    {
        if (!crystalSR) return;
        float t  = Time.time;
        var   wp = transform.position;

        // Track orb position each frame (FloatBob moves it)
        if (innerSR) innerSR.transform.position = wp;
        if (outerSR) outerSR.transform.position = wp;
        if (ringSRs != null)
            for (int i = 0; i < ringSRs.Length; i++)
                if (ringSRs[i]) ringSRs[i].transform.position = wp;

        // 1. Crystal breathes
        float tA = (Mathf.Sin(t * crystalPulseSpeed + phaseA) + 1f) * 0.5f;
        float fl = Mathf.Sin(t * flickerSpeed + phaseC) * flickerAmount;
        crystalSR.color = Color.Lerp(crystalColorA, crystalColorB, Mathf.Clamp01(tA + fl));

        // 2. Inner ambient filled circle breathes
        float tB = (Mathf.Sin(t * innerGlowSpeed + phaseB) + 1f) * 0.5f;
        innerSR.transform.localScale = Vector3.one * R2S(Mathf.Lerp(innerWorldMin, innerWorldMax, tB));
        innerSR.color = new Color(innerGlowColor.r, innerGlowColor.g, innerGlowColor.b,
                                  Mathf.Lerp(0.35f, 0.70f, tB));

        // 3. Expanding annulus pulse rings (Scale Wave sawtooth)
        for (int i = 0; i < ringCount; i++) {
            float offset   = (float)i / ringCount * ringPeriod;
            float progress = ((t + offset) % ringPeriod) / ringPeriod;
            ringSRs[i].transform.localScale = Vector3.one * R2S(Mathf.Lerp(ringWorldStart, ringWorldEnd, progress));
            ringSRs[i].color = new Color(pulseRingColor.r, pulseRingColor.g, pulseRingColor.b,
                                         ringAlphaMax * Mathf.Pow(1f - progress, ringAlphaCurve));
        }

        // 4. Outer halo breathes
        float tC = (Mathf.Sin(t * outerGlowSpeed + phaseA + 1.1f) + 1f) * 0.5f;
        outerSR.transform.localScale = Vector3.one * R2S(Mathf.Lerp(outerWorldMin, outerWorldMax, tC));
        outerSR.color = new Color(outerGlowColor.r, outerGlowColor.g, outerGlowColor.b,
                                  Mathf.Lerp(0.12f, 0.32f, tC));
    }

    void OnDestroy()
    {
        if (innerSR) Destroy(innerSR.gameObject);
        if (outerSR) Destroy(outerSR.gameObject);
        if (additiveMat) Destroy(additiveMat);
        if (ringSRs != null)
            for (int i = 0; i < ringSRs.Length; i++)
                if (ringSRs[i]) Destroy(ringSRs[i].gameObject);
    }
}