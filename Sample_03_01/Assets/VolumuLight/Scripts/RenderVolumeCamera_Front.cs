using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderVolumeCamera_Front : MonoBehaviour
{
    public RenderTexture depthTexture { get; private set; }
    Camera m_camera;
    int m_width;
    int m_height;
    // Start is called before the first frame update
    void Start()
    {
        m_width = Screen.width;
        m_height = Screen.height;
        m_camera = GetComponent<Camera>();
        depthTexture = new RenderTexture(m_width, m_height, 32, RenderTextureFormat.RFloat);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_width != Screen.width || m_height != Screen.height)
        {
            // ‰æ–Ê‰ğ‘œ“x‚ª•Ï‚í‚Á‚½‚Ì‚Åì‚è’¼‚µB
            m_width = Screen.width;
            m_height = Screen.height;
            depthTexture = new RenderTexture(m_width, m_height, 32, RenderTextureFormat.RFloat);
        }
        m_camera.targetTexture = depthTexture;
    }
}

