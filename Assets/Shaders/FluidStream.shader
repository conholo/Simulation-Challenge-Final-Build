Shader "Unlit/FluidStream"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [HDR] _ColorA ("ColorA", Color) = (1, 1, 1, 1)
        [HDR] _ColorB ("ColorB", Color) = (1, 1, 1, 1)
        _ScrollSpeed("Scroll Speed", Vector) = (1, 0, 0, 0)
        _DissolveSpeed("Dissolve Speed", Vector) = (1, 0, 0, 0)
        _DissolveScale("Dissolve Scale", float) = 50.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 500
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Cull Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 baseUV : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _DissolveScale;
            float2 _ScrollSpeed;
            float2 _DissolveSpeed;
            float4 _ColorA;
            float4 _ColorB;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.baseUV = v.uv;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            inline float random(float2 uv)
            {
                return frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453);
            }

            inline float noise_interpolate(float a, float b, float t)
            {
                return (1.0 - t) * a + (t * b);
            }

            inline float value_noise (float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                f = f * f * (3.0 - 2.0 * f);

                uv = abs(frac(uv) - 0.5);
                float2 c0 = i + float2(0.0, 0.0);
                float2 c1 = i + float2(1.0, 0.0);
                float2 c2 = i + float2(0.0, 1.0);
                float2 c3 = i + float2(1.0, 1.0);
                float r0 = random(c0);
                float r1 = random(c1);
                float r2 = random(c2);
                float r3 = random(c3);

                const float bottomOfGrid = noise_interpolate(r0, r1, f.x);
                const float topOfGrid = noise_interpolate(r2, r3, f.x);
                float t = noise_interpolate(bottomOfGrid, topOfGrid, f.y);
                return t;
            }

            //https://docs.unity3d.com/Packages/com.unity.shadergraph@7.1/manual/Simple-Noise-Node.html
            float noise(float2 uv, float scale)
            {
                float t = 0.0;
                float freq = pow(2.0, float(0));
                float amp = pow(0.5, float(3-0));
                t += value_noise(float2(uv.x*scale/freq, uv.y*scale/freq))*amp;

                freq = pow(2.0, float(1));
                amp = pow(0.5, float(3-1));
                t += value_noise(float2(uv.x*scale/freq, uv.y*scale/freq))*amp;

                freq = pow(2.0, float(2));
                amp = pow(0.5, float(3-2));
                t += value_noise(float2(uv.x * scale / freq, uv.y * scale / freq)) * amp;

                return t;
            }


            float4 frag (v2f i) : SV_Target
            {
                const float2 noiseOffset = _Time.y * _DissolveSpeed;
                const float n = noise(i.baseUV + noiseOffset, _DissolveScale);

                const float2 scrollOffset = _Time.y * _ScrollSpeed;
                const float4 tex = tex2D(_MainTex, i.baseUV + scrollOffset);

                const float inv = 1.0 - i.baseUV.r;
                const float4 mask = (n + inv - i.baseUV.x) * tex;
                
                const float4 gradient = lerp(_ColorA, _ColorB, i.baseUV.x);
                float4 result = mask * gradient;
                result.a = saturate(mask);
                clip(result.a - 0.05);
                return result;
            }
            ENDCG
        }
    }
}
