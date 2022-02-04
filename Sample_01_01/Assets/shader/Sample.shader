// シェーダー名。
Shader "Custom/Sample"
{
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

        // シェーダーモデルを指定。今回は3.0
        #pragma target 3.0

        // サーフェイスシェーダーへの入力構造体。
        struct Input
        {
            float2 uv_MainTex;  // 変数名にuv_とつけるとuv座標が引っ張ってこれるらしい。ややこしい！
        };


        // サーフェイスシェーダー。
        // Unity独特のシェーダーでサーフェイスのカラーなどのパラメータを決定する。
        // ピクセルシェーダーの中から呼ばれている関数だと考えると分かりやすい。
        // このシェーダーの後で、ライティングの計算が実行される。
        void surf (Input sin, inout SurfaceOutputStandard o)
        {
            // step-1 サーフェイスの各種パラメーターを設定する。
            
        }
        // ENDCGでシェーダープログラム終了。
        ENDCG
    }
    
    FallBack "Diffuse"
}
