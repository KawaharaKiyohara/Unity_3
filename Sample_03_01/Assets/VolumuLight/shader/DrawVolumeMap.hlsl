#ifndef _DRAWVOLUMEMAP_HLSL_
#define _DRAWVOLUMEMAP_HLSL_

#pragma vertex vert
#pragma fragment frag
#pragma enable_d3d11_debug_symbols
#include "UnityCG.cginc"

sampler2D _CameraDepthTexture;
int volumeLightID;  // 描画するボリュームライトのID。

struct appdata
{
    float4 vertex : POSITION;

};

struct v2f
{
    float4 vertex : SV_POSITION;
    float4 posInProj : TEXCOORD0;
};



v2f vert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.posInProj = o.vertex;
    return o;
}

float2 frag(v2f i) : SV_Target
{
    float2 uv = i.posInProj.xy / i.posInProj.w;
    uv *= float2(0.5f, -0.5f);
    uv += 0.5f;
    float z = tex2D(_CameraDepthTexture, uv );
    z = max(i.vertex.z, z);
    return float2(
        z, 
        (float)volumeLightID + 0.1f     // 誤差で整数部の数値が変わるとまずいので、0.1下駄をはかせる。
    );
}


#endif // _DRAWVOLUMEMAP_HLSL_  