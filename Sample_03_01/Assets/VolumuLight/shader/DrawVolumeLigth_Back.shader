Shader "VolumeLight/DrawVolume_Back"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma enable_d3d11_debug_symbols
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;

            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };



            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float frag(v2f i) : SV_Target
            {
                return i.vertex.z;
            }
            ENDCG
        }
    }
}
