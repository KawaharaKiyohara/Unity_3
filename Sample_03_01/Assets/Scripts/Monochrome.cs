using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monochrome
{
    // Start is called before the first frame update
    
    Material m_material;

    public void Start()
    {
        m_material = new Material(Shader.Find("PostEffect/Monocrome"));
    }
    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, m_material);
    }
}
