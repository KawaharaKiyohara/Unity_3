Shader "Custom/Lambert"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
     
        Pass
        {
            Tags { "LightMode" = "ForwardBase" } 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "LambertCore.hlsl"                     

            ENDCG
        }
       Pass
        {
            Tags { "LightMode" = "ForwardAdd" } 
            Blend One One // additive blending 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            #include "LambertCore.hlsl"                     

            ENDCG
        }
    }
}
