// シェーダー名。
Shader "Custom/Sample"
{    
    Properties
    {
        _albedoMap ("Albedo Map", 2D) = "white" {}
        _fadeOutMap( "Fade Out Map", 2D ) = "white" {}
        _clipParam( "Clip param", Range(0.0, 1.1) ) = 0.0
    }
    // サブシェーダー。
    SubShader
    {
        // タグの指定。
        Tags { "RenderType"="Opaque" }
        
        // ここからシェーダープログラム開始。
        CGPROGRAM

        // ここからpragmaディレクティブ。コンパイラに対する各種指定をしていく。詳細は下記URLを参照
        // https://docs.unity3d.com/ja/2018.4/Manual/SL-SurfaceShaders.html
        #pragma surface surf Standard fullforwardshadows
        // シェーダーモデルを指定。今回は3.5
        #pragma target 3.5
        // デバッグ可能にする
        #pragma enable_d3d11_debug_symbols

        sampler2D _albedoMap;   // アルベドテクスチャ

        // step-1 フェードアウト用のテクスチャにアクセスするための変数を定義する。
        sampler2D _fadeOutMap;     // フェードマップ

        float _clipParam ;
        // サーフェイスシェーダーへの入力構造体。
        // 入力構造体に渡されるデータは下記URLを参照
        // https://docs.unity3d.com/ja/2019.4/Manual/SL-SurfaceShaders.html
        struct Input
        {
            float2 uv_albedoMap;    // 変数名にuvの後ろにテクスチャ名を指定するとuv座標が引っ張ってこれるらしい。  
            float4 screenPos;
        };

        // サーフェイスシェーダー。
        void surf (Input sin, inout SurfaceOutputStandard o)
        {
            // アルベドカラーを設定する。
            o.Albedo = tex2D(_albedoMap, sin.uv_albedoMap).rgb;
            // クリップ空間から正規化スクリーン座標系に変換する。
            float2 screenPosNorm = sin.screenPos.xy / sin.screenPos.w;

            float t = tex2D( _fadeOutMap, screenPosNorm ).r;
            clip( t - _clipParam );
            
        }
        // ENDCGでシェーダープログラム終了。
        ENDCG
    }
    // Subshaderでどれもマッチしなかった時に使われるシェーダー。
    FallBack "Diffuse"
}
