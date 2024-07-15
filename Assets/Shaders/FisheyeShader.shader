Shader "Custom/FisheyeShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Strength ("Strength", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _Strength;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Transform texcoord to polar coordinates
                float2 polar = float2(atan2(i.texcoord.y, i.texcoord.x), length(i.texcoord));

                // Apply fisheye effect
                float strength = 1.0 - _Strength;
                polar.y = pow(polar.y, strength);

                // Transform back to Cartesian coordinates
                float2 cartesian = float2(polar.y * cos(polar.x), polar.y * sin(polar.x));

                // Convert back to texture coordinates
                float2 uv = cartesian * 0.5 + 0.5;

                // Sample texture
                fixed4 col = tex2D(_MainTex, uv);

                return col;
            }
            ENDCG
        }
    }
}
