Shader "VolumeLight/DrawVolume"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            Blend One One
            Cull Off
            ZWrite Off
            ZTest Always
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma enable_d3d11_debug_symbols
            #include "UnityCG.cginc"

            sampler2D volumeFrontTexture;
            sampler2D volumeBackTexture;
            float4x4 viewProjMatrixInv; // ビュープロジェクション変換の逆行列。
            float ramdomSeed;           // ランダムシード。
            int spotLightNo;
            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 posInProj : TEXCOORD0;   // 射影空間の座標。
            };
            // スポットライト
            struct SpotLight
            {
                float3 position;        // 座標
                int isUse;              // 使用中フラグ。
                float3 positionInView;  // カメラ空間での座標。
                int no ;                // ライトの番号。
                float3 direction;       // 射出方向。
                float3 range;           // 影響範囲。
                float3 color;           // ライトのカラー。
                float3 color2;          // 二つ目のカラー。
                float3 color3;          // 三つ目のカラー。
                float3 directionInView; // カメラ空間での射出方向。
                float3 rangePow;        // 距離による光の影響率に累乗するパラメーター。1.0で線形の変化をする。
                                        // xが一つ目のカラー、yが二つ目のカラー、zが三つ目のカラー。
                float3 angle;           // 射出角度(単位：ラジアン。xが一つ目のカラー、yが二つ目のカラー、zが三つ目のカラー)。
                float3 anglePow;        // スポットライトとの角度による光の影響率に累乗するパラメータ。1.0で線形に変化する。
                                        // xが一つ目のカラー、yが二つ目のカラー、zが三つ目のカラー。
            };

            StructuredBuffer< SpotLight> volumeSpotLightArray;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = v.vertex;
                o.vertex.y *= -1.0f;
                o.vertex.z = 1.0f;
                o.vertex.w = 1.0f;
                o.posInProj = o.vertex;
                return o;
            }
            /*!
             * @brief	UV座標と深度値からワールド座標を計算する。
             * @param[in]	uv				uv座標
             * @param[in]	zInScreen		スクリーン座標系の深度値
             * @param[in]	mViewProjInv	ビュープロジェクション行列の逆行列。
             */
            float3 CalcWorldPosFromUVZ(float2 uv, float zInScreen, float4x4 mViewProjInv)
            {
                float3 screenPos;
                screenPos.xy = (uv * float2(2.0f, -2.0f)) + float2(-1.0f, 1.0f);
                screenPos.z = zInScreen;

                float4 worldPos = mul(mViewProjInv, float4(screenPos, 1.0f));
                worldPos.xyz /= worldPos.w;
                return worldPos.xyz;
            }
            float GetRandomNumber(float2 texCoord, float Seed)
            {
                return frac(sin(dot(texCoord.xy, float2(12.9898, 78.233)) + Seed) * 43758.5453);
            }
            fixed4 frag(v2f i) : SV_Target
            {
                
                float2 uv = i.posInProj.xy / i.posInProj.w;
                uv *= float2(0.5f, -0.5f);
                uv += 0.5f;
                
                // 
                float2 volumeFrontZ_ID = tex2D(volumeFrontTexture, uv).rg;
                float2 volumeBackZ_ID = tex2D(volumeBackTexture, uv).rg;
                
                float3 volumePosBack = CalcWorldPosFromUVZ(uv, volumeBackZ_ID.r, viewProjMatrixInv);
                float3 volumePosFront = CalcWorldPosFromUVZ(uv, volumeFrontZ_ID.r, viewProjMatrixInv);

                SpotLight spotLight = volumeSpotLightArray[spotLightNo];
                float t0 = dot(spotLight.direction, volumePosFront - spotLight.position);
                float t1 = dot(spotLight.direction, volumePosBack - spotLight.position);
                
                float t = t0 / (t0 + t1);
                float3 volumeCenterPos = lerp(volumePosFront, volumePosBack, t);
                float volume = length(volumePosBack - volumePosFront);

                // ボリュームがない箇所はピクセルキル。
                clip(volume - 0.001f);
                
                // float4 albedoColor = albedoTexture.Sample(Sampler, uv);
                float4 albedoColor = float4( 0.5f, 0.5f, 0.5f, 1.0f);

                // 距離による光の影響率を計算。
                float3 ligDir = (volumeCenterPos - spotLight.position);
                float distance = length(ligDir);
                ligDir = normalize(ligDir);
                float3 affectBase = 1.0f - min(1.0f, distance / spotLight.range);
                float3 affect = pow( affectBase, spotLight.rangePow);     

                // 続いて角度による減衰を計算する。
                // 角度に比例して小さくなっていく影響率を計算する
                float angleLigToPixel = saturate(dot(ligDir, spotLight.direction) );
                
                // dot()で求めた値をacos()に渡して角度を求める
                angleLigToPixel = abs(acos(angleLigToPixel)) ;
                
                // 光の角度による減衰を計算。
                float3 angleAffectBase = max( 0.0f, 1.0f - 1.0f / spotLight.angle * angleLigToPixel );
                angleAffectBase = min( 1.0f, angleAffectBase * 1.8f);
                float3 angleAffect = pow( angleAffectBase, spotLight.anglePow );    
                affect *= angleAffect;

                float3 lig = 0;
                // 三つの光を合成。    
                // 光のベースを計算。
                float3 ligBase = albedoColor * step( volumeFrontZ_ID.r, albedoColor.w ) * max( 0.0f, log(volume) ) * 0.1f;
                // 光のベースに影響率を乗算する。
                lig = ligBase * affect.x * spotLight.color; 
                lig += ligBase * affect.y * spotLight.color2;
                lig += ligBase * affect.z * spotLight.color3;
                
                // 空気中のチリの表現としてノイズを加える。
                lig *= lerp( 0.9f, 1.1f, GetRandomNumber(uv, ramdomSeed));
                // lig *= lerp( 0.9f, 1.1f, 0.0f);

                return fixed4( lig, 1.0f);
            }
            ENDCG
        }
    }
}
