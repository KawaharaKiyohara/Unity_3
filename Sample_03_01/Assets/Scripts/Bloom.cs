using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloom 
{
    // Start is called before the first frame update
    RenderTexture m_luminanceTexture;      // 輝度が書き込まれるテクスチャ;
    Material m_luminanceMaterial;          // 輝度抽出用のマテリアル
    Material m_copyMaterial;               // コピー用のマテリアル。
    
    public void Start()
    {       
        m_luminanceMaterial = new Material(Shader.Find("PostEffect/Luminance"));
        m_copyMaterial = new Material(Shader.Find("PostEffect/Copy"));
    }
    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        RenderTexture luminanceTexture = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        Graphics.Blit(source, luminanceTexture, m_luminanceMaterial);
        Graphics.Blit(luminanceTexture, destination, m_copyMaterial);
    }

}
