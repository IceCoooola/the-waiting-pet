Shader "Custom/PixelWater"
{
    Properties
    {
        _BaseColor      ("Base Color",         Color) = (0.08, 0.20, 0.28, 0.92)
        _DeepColor      ("Deep Tint",          Color) = (0.04, 0.10, 0.16, 0.95)
        _HighlightColor ("Highlight",          Color) = (0.35, 0.60, 0.75, 0.50)
        _GlowColor      ("Glow Pulse",         Color) = (0.08, 0.28, 0.45, 1.00)
        _OverlayColor   ("Shadow Overlay",     Color) = (0.00, 0.04, 0.08, 0.55)

        _NoiseScaleA    ("Noise Scale A",      Float) = 3.5
        _NoiseSpeedAx   ("Noise Speed A x",   Float) = 0.002
        _NoiseSpeedAy   ("Noise Speed A y",   Float) = 0.001

        _NoiseScaleB    ("Noise Scale B",      Float) = 6.2
        _NoiseSpeedBx   ("Noise Speed B x",   Float) = -0.0015
        _NoiseSpeedBy   ("Noise Speed B y",   Float) = 0.0008

        _CausticScale   ("Caustic Scale",      Float) = 5.0
        _CausticSpeed   ("Caustic Speed",      Float) = 0.002

        _HighlightScale ("Highlight Scale",    Float) = 6.5
        _HighlightSpeed ("Highlight Speed",    Float) = 0.008
        _HighlightThresh("Highlight Thresh",   Float) = 0.80
        _PixelSnap      ("Pixel Snap",         Float) = 8.0

        _DepthPow       ("Depth Power",        Float) = 1.2
        _GlowSpeed      ("Glow Cycle Speed",   Float) = 0.065
        _GlowIntensity  ("Glow Intensity",     Float) = 0.045

        _RippleAmp      ("Ripple Amp",         Float) = 0.0
        _RippleCenter   ("Ripple Center",      Vector) = (0.5, 0.9, 0, 0)
        _RippleTime     ("Ripple Time",        Float) = -99.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f    { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };

            float4 _BaseColor, _DeepColor, _HighlightColor, _GlowColor, _OverlayColor;
            float  _NoiseScaleA, _NoiseSpeedAx, _NoiseSpeedAy;
            float  _NoiseScaleB, _NoiseSpeedBx, _NoiseSpeedBy;
            float  _CausticScale, _CausticSpeed;
            float  _HighlightScale, _HighlightSpeed, _HighlightThresh, _PixelSnap;
            float  _DepthPow, _GlowSpeed, _GlowIntensity;
            float  _RippleAmp, _RippleTime;
            float4 _RippleCenter;

            float hash(float2 p) {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453);
            }
            float2 psnap(float2 uv, float s) {
                return floor(uv * s + 0.5) / s;
            }

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float  t  = _Time.y;

                // Depth gradient — subtle, not extreme
                // uv.y=0 bottom, uv.y=1 top (surface)
                float depthT = pow(saturate(1.0 - uv.y), _DepthPow);
                float4 col   = lerp(_BaseColor, _DeepColor, depthT * 0.5);

                // Depth also damps motion so deep water barely moves
                float motionDamp = 1.0 - depthT * 0.80;

                // Two noise layers — multiplied to break tiling
                float2 uvA = psnap(uv * _NoiseScaleA
                           + float2(t * _NoiseSpeedAx, t * _NoiseSpeedAy) * motionDamp, _PixelSnap);
                float2 uvB = psnap(uv * _NoiseScaleB
                           + float2(t * _NoiseSpeedBx, t * _NoiseSpeedBy) * motionDamp, _PixelSnap);
                float  nA  = hash(uvA);
                float  nB  = hash(uvB);
                float  noise = nA * nB;
                // Apply as extremely subtle brightness variation
                col.rgb   += (noise - 0.5) * 0.025 * motionDamp;

                // Caustics: barely perceptible (almost still water)
                float2 cUV  = uv * _CausticScale
                            + float2(t * _CausticSpeed, t * _CausticSpeed * 0.55) * motionDamp;
                float  cau  = sin(cUV.x + sin(cUV.y + t * _CausticSpeed * 2.5)) * 0.5 + 0.5;
                col.rgb    += step(0.74, cau) * 0.05 * (1.0 - depthT * 0.7) * motionDamp;

                // Ripple from splash
                if (_RippleTime > -10.0) {
                    float rt    = t - _RippleTime;
                    float dist  = abs(uv.x - _RippleCenter.x);
                    float rwave = sin(dist * 26.0 - rt * 12.0)
                                * exp(-rt * 2.5) * exp(-dist * 5.5)
                                * _RippleAmp * 0.04;
                    col.rgb += rwave * _HighlightColor.rgb;
                }

                // Pixel-art sparkle highlights — very sparse, very slow
                float2 h1 = psnap(uv * _HighlightScale
                          + float2( t * _HighlightSpeed,       t * _HighlightSpeed * 0.3),  _PixelSnap);
                float2 h2 = psnap(uv * _HighlightScale * 0.55
                          - float2( t * _HighlightSpeed * 0.4, t * _HighlightSpeed * 0.2),  _PixelSnap);
                float spark = step(_HighlightThresh,        hash(h1)) * 0.35
                            + step(_HighlightThresh + 0.05, hash(h2)) * 0.20;
                col.rgb    += _HighlightColor.rgb * spark * (1.0 - depthT * 0.6);

                // Magical glow pulse — 8-15 second cycle, barely noticeable
                float glow = (sin(t * _GlowSpeed * 6.28318) * 0.5 + 0.5) * _GlowIntensity;
                col.rgb   += _GlowColor.rgb * glow * (1.0 - depthT * 0.4);

                // Shadow overlay — applied uniformly to darken + add atmosphere
                col.rgb = lerp(col.rgb, _OverlayColor.rgb, _OverlayColor.a * 0.6);

                // Uniform alpha — no surface transparency tricks
                col.a = lerp(_BaseColor.a, _DeepColor.a, depthT);

                return saturate(col);
            }
            ENDCG
        }
    }
}
