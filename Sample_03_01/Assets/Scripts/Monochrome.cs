using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monochrome : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    Material m_material;

    void Start()
    {
        Camera camera = GetComponent<Camera>();
        camera.depthTextureMode = DepthTextureMode.Depth;
    }
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, m_material);
    }
}
