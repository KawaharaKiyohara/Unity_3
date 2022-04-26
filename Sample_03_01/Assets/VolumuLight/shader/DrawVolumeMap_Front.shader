Shader "VolumeLight/DrawVolume_Front"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Cull Back

            CGPROGRAM

            #include "DrawVolumeMap.hlsl"

            ENDCG
        }
    }
}
