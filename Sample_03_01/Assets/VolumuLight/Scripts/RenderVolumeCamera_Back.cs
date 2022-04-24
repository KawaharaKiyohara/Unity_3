using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderVolumeCamera_Back : MonoBehaviour
{
    public RenderTexture depthTexture { get; private set; }
    Camera m_camera;
    int m_width;
    int m_height;
    // Start is called before the first frame update
    void Start()
    {
        m_camera = GetComponent<Camera>();
        m_width = Screen.width;
        m_height = Screen.height;
        depthTexture = new RenderTexture(m_width, m_height, 32, RenderTextureFormat.RFloat);

    }

    // Update is called once per frame
    void Update()
    {
        if(m_width != Screen.width || m_height != Screen.height)
        {
            // 画面解像度が変わったので作り直し。
            m_width = Screen.width;
            m_height = Screen.height;
            depthTexture = new RenderTexture(m_width, m_height, 32, RenderTextureFormat.RFloat);
        }
        
        m_camera.targetTexture = depthTexture;
    }
}
