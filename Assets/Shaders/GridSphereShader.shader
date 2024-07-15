Shader "Custom/TransparentGridSphereShader"
{
    Properties
    {
        _GridColor ("Grid Color", Color) = (0, 0, 0, 1)
        _BackgroundColor ("Background Color", Color) = (1, 1, 1, 0)
        _GridSize ("Grid Size", Float) = 10.0
        _LineWidth ("Line Width", Float) = 0.02
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 100

        Pass
        {
            Cull Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 localPos : TEXCOORD0;
            };

            float4 _GridColor;
            float4 _BackgroundColor;
            float _GridSize;
            float _LineWidth;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex.xyz; // Save local position
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Normalize local position to get world position
                float3 worldPos = normalize(i.localPos);
                float theta = acos(worldPos.y); // polar angle
                float phi = atan2(worldPos.z, worldPos.x); // azimuthal angle

                // Scale coordinates to create a grid pattern
                float s = theta / 3.1415926 * _GridSize;
                float t = (phi + 3.1415926) / (2.0 * 3.1415926) * _GridSize;

                // Create a grid effect
                float2 grid = frac(float2(s, t));
                float2 gridLines = smoothstep(0.0, _LineWidth, min(grid, 1.0 - grid));

                // Combine grid lines to create the final pattern
                float gridPattern = gridLines.x * gridLines.y;

                // Blend grid color with background color based on grid pattern
                fixed4 color = lerp(_BackgroundColor, _GridColor, gridPattern);

                return color;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
