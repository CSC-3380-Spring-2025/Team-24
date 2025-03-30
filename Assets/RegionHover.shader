Shader "Custom/RegionHover"
{
    Properties
    {
        _MainTex ("Color Map", 2D) = "white" {}
        _MaskTex ("Region Mask", 2D) = "black" {}
        _HighlightColor ("Highlight Color", Color) = (1,1,1,0.5)
        _HoverRegion ("Hover Region", Float) = -1
    }

    SubShader
    {
        Tags { "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

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

            sampler2D _MainTex, _MaskTex;
            float4 _HighlightColor;
            float _HoverRegion;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 mapColor = tex2D(_MainTex, i.uv);
                float maskValue = tex2D(_MaskTex, i.uv).r;

                // Only highlight if mask matches a VALID region value exactly
                float shouldHighlight = abs(maskValue - _HoverRegion) < 0.001;
                return lerp(mapColor, _HighlightColor, shouldHighlight);
            }
            ENDCG
        }
    }
}