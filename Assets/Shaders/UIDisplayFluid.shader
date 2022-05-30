Shader "Unlit/UIDisplayFluid"
{
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            Texture2D<float4> _MaskTexture;
            SamplerState sampler_MaskTexture;
            
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                float4 maskColor = _MaskTexture.SampleLevel(sampler_MaskTexture, i.uv, 0);

                col.a = dot(col.rgb, col.rgb) / 3.0 == 1.0 ? 0.0 : col.a;
                col = dot(col.rgb, col.rgb) < 3.0 && col.a < 1.0 ? maskColor : col;
                clip(col.a - 0.5);
                return col;
            }
            ENDCG
        }
    }
}
