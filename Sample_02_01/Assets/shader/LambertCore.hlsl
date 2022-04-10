#ifndef _LAMBERT_CORE_H_
#define _LAMBERT_CORE_H_
struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    float3 normal : NORMAL;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
    float3 normal : NORMAL;
};

sampler2D _MainTex;
float4 _MainTex_ST;


v2f vert (appdata v)
{
    v2f o;
    
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.normal = mul(unity_ObjectToWorld, v.normal );
    return o;
}
fixed4 frag(v2f i) : SV_Target0
{
    
    fixed4 col = tex2D(_MainTex, i.uv);

    
    fixed4 lit = max( 0.0, dot(i.normal, _WorldSpaceLightPos0.xyz) );
    
    lit *= _LightColor0 ;
    
    col.xyz *= lit.xyz;
    return col;
  
  
}
#endif // _LAMBERT_CORE_H_