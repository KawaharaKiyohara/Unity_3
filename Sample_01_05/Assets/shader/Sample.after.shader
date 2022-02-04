// シェーダー名。
Shader "Custom/Sample_after"
{    
    Properties
    {
        // _albedoMapをインスペクタに公開。
        _albedoMap ("Albedo Map", 2D) = "white"
        // _metallicSmoothMapをインスペクタに公開。
        _metallicSmoothMap("Metallic Smooth Map", 2D ) = "gray"
        // step-2 _normalMapをインスペクタに公開。
        _normalMap("Normal Map", 2D ) = "gray"
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
        // step-1 法線マップにアクセスするための変数を定義する。
        sampler2D _normalMap;       
        
        // サーフェイスシェーダーへの入力構造体。
        struct Input
        {
            float2 uv_albedoMap;  // 変数名にuvの後ろにテクスチャ名を指定するとuv座標が引っ張ってこれるらしい。ややこしい！
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

            // step-3 法線マップから法線を計算する。
            o.Normal = UnpackNormal( tex2D( _normalMap, sin.uv_albedoMap ) );
        }
        // ENDCGでシェーダープログラム終了。
        ENDCG
    }
    // Subshaderでどれもマッチしなかった時に使われるシェーダー。
    FallBack "Diffuse"
}
