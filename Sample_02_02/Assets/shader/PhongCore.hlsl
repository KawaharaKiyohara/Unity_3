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
    float3 posInWorld : TEXCOORD1;
};

sampler2D _MainTex;
float4 _MainTex_ST;


v2f vertCore (appdata v)
{
    v2f o;
    
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    o.normal = mul(unity_ObjectToWorld, v.normal );
    // ワールド座標を計算する。
    o.posInWorld = mul(unity_ObjectToWorld, v.vertex );
    return o;
}
fixed4 fragCore(v2f i, uniform bool useAmbient) 
{
    
    fixed4 col = tex2D(_MainTex, i.uv);
    
    // ランバート拡散反射
    fixed4 lit = max( 0.0, dot(normalize(i.normal), _WorldSpaceLightPos0.xyz) );
    // フォン鏡面反射
    float3 litDirRef = reflect(-_WorldSpaceLightPos0.xyz, i.normal);
    // カメラまでのベクトルを計算する。
    float3 toEye = normalize(_WorldSpaceCameraPos - i.posInWorld);
    lit += max( 0.0,dot( toEye, litDirRef) );

    if( useAmbient){
        lit += unity_AmbientSky;
    }
    lit *= _LightColor0 ;
    col.xyz *= lit.xyz;
    return col;
  
}
#endif // _LAMBERT_CORE_H_