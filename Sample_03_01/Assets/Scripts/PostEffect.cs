using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostEffect : MonoBehaviour
{
    Bloom m_bloom;              // ブルーム
    Monochrome m_monochrome;    // モノクロ
    RenderTexture[] m_temporaryRt = new RenderTexture[2];
    Material m_copyMaterial;
    // Start is called before the first frame update
    void Start()
    {
        Camera cam = GetComponent<Camera>();
        
        m_bloom = new Bloom();
        m_monochrome = new Monochrome();
        m_bloom.Start();
        m_monochrome.Start();

        
        m_copyMaterial = new Material(Shader.Find("PostEffect/Copy"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        m_temporaryRt[0] = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        m_temporaryRt[1] = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
        m_bloom.OnRenderImage(source, m_temporaryRt[0]);
        m_monochrome.OnRenderImage(m_temporaryRt[0], m_temporaryRt[1]);
        // 最終結果をコピー。
        Graphics.Blit(m_temporaryRt[1], destination, m_copyMaterial);

    }
}
