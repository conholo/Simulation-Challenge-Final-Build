
Shader "Unlit/Fluid"
{
    Properties
    {
        _WobbleZ("Wobble Z", range(-1.0, 1.0)) = 0
        _WobbleX("Wobble X", range(-1.0, 1.0)) = 0
        _WobblePower("Wobble Power", float) = 1
        _FillPercent("Fill Percent", range(0, 1)) = 0
        _FresnelStrength("Fresnel Strength", float) = 1.0
        _SideColor("Side Color", Color) = (1, 1, 1, 1)
        _TopColor("Top Color", Color) = (1, 1, 1, 1)
        _FresnelColor("Fresnel Color", Color) = (1, 1, 1, 1)
        _RemapFillMin("Remap Fill Min", float) = -1.0
        _RemapFillMax("Remap Fill Max", float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Off

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 vertexWorldPosition : TEXCOORD2;
                float3 objectWorldPosition : TEXCOORD3;
                float3 worldSpaceViewDirection : TEXCOORD4;
                float3 worldSpaceNormal : TEXCOORD5;
                float3 wobble : TEXCOORD6;
                float3 objectVertexPosition : TEXCOORD7;
                float3 scale : TEXCOORD8;
            };

            #define PI 3.14159265359
            
            float4 _SideColor;
            float4 _TopColor;
            float4 _FresnelColor;
            float _FillPercent;
            float _FresnelStrength;
            float _WobbleX;
            float _WobbleZ;
            float _WobblePower;
            float _RemapFillMin;
            float _RemapFillMax;

            float3 rotate_about_axis(float3 v, float3 axis, float rotation)
            {
                float s = sin(rotation);
                float c = cos(rotation);
                const float one_minus_c = 1.0 - c;

                axis = normalize(axis);
                const float3x3 rotationMatrix = 
                {   one_minus_c * axis.x * axis.x + c, one_minus_c * axis.x * axis.y - axis.z * s, one_minus_c * axis.z * axis.x + axis.y * s,
                    one_minus_c * axis.x * axis.y + axis.z * s, one_minus_c * axis.y * axis.y + c, one_minus_c * axis.y * axis.z - axis.x * s,
                    one_minus_c * axis.z * axis.x - axis.y * s, one_minus_c * axis.y * axis.z + axis.x * s, one_minus_c * axis.z * axis.z + c
                };
                return mul(rotationMatrix, v);
            }

            float degrees_to_rads(float d)
            {
                return d * 180.0 / PI;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // Rotation of 90 degrees.
                const float rads = degrees_to_rads(90.0);
                // Apply rotation around the X axis, multiplied by the current wobbleX provided by the user.
                const float3 wobbleX = rotate_about_axis(v.vertex, float3(1.0, 0.0, 0.0), rads) * _WobbleX;
                // Apply rotation around the Z axis, multiplied by the current wobbleZ provided by the user.
                const float3 wobbleZ = rotate_about_axis(v.vertex, float3(0.0, 0.0, 1.0), rads) * _WobbleZ;
                // Store the result of the net wobble.
                o.wobble = wobbleX - wobbleZ;

                float3 scale = float3(
                    length(unity_ObjectToWorld._m00_m10_m20),
                    length(unity_ObjectToWorld._m01_m11_m21),
                    length(unity_ObjectToWorld._m02_m12_m22)
                );

                o.scale = scale;
                // Store various positional data to be interpolated to the fragment shader.
                o.objectVertexPosition = v.vertex;
                o.worldSpaceViewDirection = WorldSpaceViewDir(v.vertex);
                o.vertexWorldPosition = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0)).xyz;
                o.objectWorldPosition = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));
                o.worldSpaceNormal = mul(unity_ObjectToWorld, float4(v.normal, 0.0)).xyz;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }
            
            float remap(float a0, float a1, float b0, float b1, float v)
            {
                return b0 + (v - a0) * (b1 - b0) / (a1 - a0);
            }

            float fresnel(float3 n, float3 v, float p)
            {
                return pow(1.0 - saturate(dot(normalize(n), normalize(v))), p);
            }

            fixed4 frag (v2f i, fixed facing: VFACE) : SV_Target
            {
                // Relative offset from object to vertex - world space.
                const float3 diff = (i.vertexWorldPosition - i.objectWorldPosition) / i.scale;
                // Relative distance to the center of the model, pow'd to multiply the effect towards the edges.
                const float d = pow(length(diff), _WobblePower);
                // The yOffset, or height of the current fragment relative to the container.
                const float yOffset = diff.y + i.wobble.y * d;
                // Remapping the fill percent so that it fits the current model.
                const float fill = remap(0.0, 1.0, _RemapFillMin, _RemapFillMax, _FillPercent);

                // If the fill <= height of the fragment, a = 0.0, otherwise 1.0
                float a = step(yOffset, fill);

                // If the fragment is front facing, use the side color, otherwise the top color.  Culling Off is necessary here.
                float4 color = facing > 0.0 ? float4(_SideColor.rgb, a) : float4(_TopColor.rgb, a);
                // Adding Fresnel shading.
                const float3 fresnelColor = _FresnelColor * fresnel(i.worldSpaceNormal, i.worldSpaceViewDirection, _FresnelStrength);
                color.rgb += fresnelColor;

                // Alpha clip at 0.5
                clip(color.a - 0.5);
                UNITY_APPLY_FOG(i.fogCoord, color);

                return color;
            }
            ENDCG
        }
    }
}
