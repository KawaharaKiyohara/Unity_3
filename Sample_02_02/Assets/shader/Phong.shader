Shader "Custom/Phong"
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
            #include "PhongCore.hlsl"          

            v2f vert (appdata v)
            {
                return vertCore(v);
            }
            fixed4 frag(v2f i) : SV_Target0
            {
                return fragCore(i, true);
            }
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

            #include "PhongCore.hlsl"                     

            v2f vert (appdata v)
            {
                return vertCore(v);
            }

            fixed4 frag(v2f i) : SV_Target0
            {
                return fragCore(i, false);
            }
            ENDCG
        }
    }
}
