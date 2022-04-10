// シェーダー名。
Shader "Custom/Sample"
{    
    Properties
    {
        _albedoMap ("Albedo Map", 2D) = "white" {}
        _metallicSmoothMap("Metallic Smooth Map", 2D ) = "gray" {}
        _normalMap("Normal Map", 2D ) = "gray" {}
        // step-2 リムライトの調整パラメーターをインスペクタに公開する。
        _limColor ("Lim Color", Color ) = (0.5, 0.5, 0.5, 1.0) 
        _limIntensity ("Lim Intensity", Range( 0.0, 10.0) ) = 5.0 
        _limPowerPow ("Lim PowerPow", Range( 0.0, 10.0) ) = 1.0
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

        sampler2D _albedoMap;          // アルベドテクスチャ
        sampler2D _metallicSmoothMap;  // メタリックスムースマップ。        
        sampler2D _normalMap;          // 法線マップ。
        
        // step-1 リムライトを調整するためのパラメータにアクセスする変数を定義する。
        uniform float4 _limColor;           // リムのカラー。
        uniform float _limIntensity;        // リムの強度。値が大きいほどリムライトのカラーが強くなります。
        uniform float _limPowerPow;         // リムライトの強さに累乗する値。値が大きいほどリムライトの影響を絞れます。
        
        // サーフェイスシェーダーへの入力構造体。
        // 入力構造体に渡されるデータは下記URLを参照
        // https://docs.unity3d.com/ja/2019.4/Manual/SL-SurfaceShaders.html
        struct Input
        {
            float2 uv_albedoMap;    // 変数名にuvの後ろにテクスチャ名を指定するとuv座標が引っ張ってこれるらしい。ややこしい！
            // step-3 カメラに向かうベクトルを追加する。
            float3 viewDir;         // カメラの方向。
        };

        // サーフェイスシェーダー。
        void surf (Input sin, inout SurfaceOutputStandard o)
        {
            // アルベドカラーを設定する。
            o.Albedo = tex2D(_albedoMap, sin.uv_albedoMap).rgb;
            // テクスチャからサンプリングしてメタリックとスムースを設定する。
            fixed4 metallicSmooth = tex2D(
                _metallicSmoothMap, 
                sin.uv_albedoMap       // UV座標はアルベドと一緒でいいので、uv_albedoMapを使い回す。
            );
            // ライティングモデルがUnityのスタンダードシェーダーはメタリックワークフローなので、
            // Unityちゃんのスペキュラマップには、rチャンネルに金属度、aに滑らかさが入っている・・・はず。
            o.Metallic = metallicSmooth.r;
            o.Smoothness = metallicSmooth.a;

            // 法線マップから法線を計算する。
            o.Normal = UnpackNormal( tex2D( _normalMap, sin.uv_albedoMap ) );

            // step-4 視線の方向に依存するリムライトの強さを計算する。   
            // 視点へのベクトルと法線とで内積を計算する。
            float en = dot( normalize( sin.viewDir ), o.Normal);
            // リムライトのベースの強さを計算する。
            float limBasePower = 1.0 - abs(en) ;
            // ベースの強さをn乗して最終的な強さを絞る。
            float limPower = pow( limBasePower, _limPowerPow ) * _limIntensity ;

            // step-5 リムカラーをアルベドカラーに足し算する。
            // リムをアルベドカラーに足し算。
            o.Albedo += limPower * _limColor;
        }
        // ENDCGでシェーダープログラム終了。
        ENDCG
    }
    // Subshaderでどれもマッチしなかった時に使われるシェーダー。
    FallBack "Diffuse"
}
