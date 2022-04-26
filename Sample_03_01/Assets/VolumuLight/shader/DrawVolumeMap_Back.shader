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

            #include "DrawVolumeMap.hlsl"

            ENDCG
        }
    }
}
