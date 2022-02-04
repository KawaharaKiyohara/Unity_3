// シェーダー名。
Shader "Custom/Sample_after"
{
    // step-3 テクスチャをインスペクタから設定できるように外部に公開する。
    Properties
    {
        // 公開したい変数名 ("インスペクタに表示される名前"　データの種類) = "初期値"
        _albedo ("Albedo Map", 2D) = "white"
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
        // サーフェイスシェーダーのライティングモデルを指定する。
        // Standardを指定でUnityの通常のPBRシェーダーが使われる。(Standard shader)
        // fullforwardshadowsを指定することで、フォワードレンダリングのパスで描画され、
        // 全てのライトシャドウがサポートされる。
        #pragma surface surf Standard fullforwardshadows

        // シェーダーモデルを指定。今回は3.5
        // OpenGL ES 3.0 機能 (D3D プラットフォームの DX10 SM4.0、ジオメトリシェーダーのみ含まれない)
        // DX11 9.x (WinPhone) と OpenGL ES 2.0 ではサポートされません。
        // DX11 以降、OpenGL 3.2 以降、OpenGL ES 3 以降、Metal、Vulkan,、PS4/XB1 コンソールでサポートされます。
        // シェーダー、テクスチャ配列などのネイティブの整数演算
        #pragma target 3.5

        // step-1 テクスチャにアクセスするための変数を定義する。
        // smpler2Dはテクスチャとサンプラをまとめて定義できる。古いグラフィクスAPI(OpenGLES)で唯一サポートされている選択肢
        sampler2D _albedo;
        
        // サーフェイスシェーダーへの入力構造体。
        struct Input
        {
            float2 uv_albedo;  // 変数名にuvの後ろにテクスチャ名を指定するとuv座標が引っ張ってこれるらしい。ややこしい！
        };


        // サーフェイスシェーダー。
        // Unity独特のシェーダーでサーフェイスのカラーなどのパラメータを決定する。
        // ピクセルシェーダーの中から呼ばれている関数だと考えると分かりやすい。
        // このシェーダーの後で、ライティングの計算が実行される。
        void surf (Input sin, inout SurfaceOutputStandard o)
        {
            // step-2 テクスチャからサンプリングしてアルベドカラーを設定する。
            o.Albedo = tex2D(_albedo, sin.uv_albedo).rgb;
            o.Metallic = 0.5f;
            o.Smoothness = 0.5f;

        }
        // ENDCGでシェーダープログラム終了。
        ENDCG
    }
    // Subshaderでどれもマッチしなかった時に使われるシェーダー。
    // 今は気にしなくてよい。
    FallBack "Diffuse"
}
