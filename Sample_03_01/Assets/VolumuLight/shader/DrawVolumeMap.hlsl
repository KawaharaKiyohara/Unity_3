#ifndef _DRAWVOLUMEMAP_HLSL_
#define _DRAWVOLUMEMAP_HLSL_

#pragma vertex vert
#pragma fragment frag
#pragma enable_d3d11_debug_symbols
#include "UnityCG.cginc"


int volumeLightID;  // 描画するボリュームライトのID。

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

float2 frag(v2f i) : SV_Target
{
    return float2(
        i.vertex.z, 
        (float)volumeLightID + 0.1f     // 誤差で整数部の数値が変わるとまずいので、0.1下駄をはかせる。
    );
}


#endif // _DRAWVOLUMEMAP_HLSL_  