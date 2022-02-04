// シェーダー名。
Shader "Custom/Sample_after"
{    
    Properties
    {
        // _albedoをインスペクタに公開。
        _albedo ("Albedo Map", 2D) = "white"
        // step-2 _metallicSmoothをインスペクタに公開。
        _metallicSmooth("Metallic Smooth Map", 2D ) = "gray"
    }
    // サブシェーダー。今は気にしなくてよい。
    SubShader
    {
        // タグの指定。
        // RenderTypeはシェーダーのグループ化を行える。
        // Opaqueは不透明グループ
        Tags { "RenderType"="Opaque" }
        
        // ここからシェーダープログラム開始。
        CGPROGRAM

        // ここからpragmaディレクティブ。コンパイラに対する各種指定をしていく。詳細は下記URLを参照
        // https://docs.unity3d.com/ja/2018.4/Manual/SL-SurfaceShaders.html

        #pragma surface surf Standard fullforwardshadows

        // シェーダーモデルを指定。今回は3.5
        #pragma target 3.5

        sampler2D _albedo;          // アルベドテクスチャ
        // step-1 メタリックスムースマップにアクセスするための変数を定義。
        sampler2D _metallicSmooth;  // メタリックスムースマップ。
        
        // サーフェイスシェーダーへの入力構造体。
        struct Input
        {
            float2 uv_albedo;  // 変数名にuvの後ろにテクスチャ名を指定するとuv座標が引っ張ってこれるらしい。ややこしい！
        };

        // サーフェイスシェーダー。
        void surf (Input sin, inout SurfaceOutputStandard o)
        {
            // アルベドカラーを設定する。
            o.Albedo = tex2D(_albedo, sin.uv_albedo).rgb;
            // step-3 テクスチャからサンプリングしてメタリックとスムースを設定する。
            fixed4 metallicSmooth = tex2D(
                _metallicSmooth, 
                sin.uv_albedo       // UV座標はアルベドと一緒でいいので、uv_albedoを使い回す。
            );
            // ライティングモデルがUnityのスタンダードシェーダーはメタリックワークフローなので、
            // Unityちゃんのスペキュラマップには、rチャンネルに金属度、aに滑らかさが入っている・・・はず。
            o.Metallic = metallicSmooth.r;
            o.Smoothness = metallicSmooth.a;
        }
        // ENDCGでシェーダープログラム終了。
        ENDCG
    }
    // Subshaderでどれもマッチしなかった時に使われるシェーダー。
    // 今は気にしなくてよい。
    FallBack "Diffuse"
}
