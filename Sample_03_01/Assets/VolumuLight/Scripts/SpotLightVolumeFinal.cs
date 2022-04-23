using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotLightVolumeFinal : MonoBehaviour
{
    // Start is called before the first frame update
    Material m_material;
    Camera volumeFrontCamera;
    Camera volumeBackCamera;
    int volumeFrontTexID;
    int volumeBackTexID;
    int viewProjectionMatrixInvID;
    int randomSeedID;
    void Start()
    {
        var mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>(); ;
        mainCamera.depthTextureMode = DepthTextureMode.Depth;
        m_material = GetComponent<MeshRenderer>().material;
        volumeFrontCamera = GameObject.Find("VolumeRenderCamera_front").GetComponent<Camera>(); ;
        volumeBackCamera = GameObject.Find("VolumeRenderCamera_back").GetComponent<Camera>(); ;
        volumeFrontTexID = Shader.PropertyToID("volumeFrontTexture");
        volumeBackTexID = Shader.PropertyToID("volumeBackTexture");
        viewProjectionMatrixInvID = Shader.PropertyToID("viewProjMatrixInv");
        randomSeedID = Shader.PropertyToID("ramdomSeed");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void LateUpdate()
    {
        var projMatrix = GL.GetGPUProjectionMatrix(volumeFrontCamera.projectionMatrix, true);
        Matrix4x4 mViewProjMatInv = projMatrix * volumeFrontCamera.worldToCameraMatrix;
        mViewProjMatInv = Matrix4x4.Inverse(mViewProjMatInv);
        m_material.SetTexture(volumeFrontTexID, volumeFrontCamera.targetTexture);
        m_material.SetTexture(volumeBackTexID, volumeBackCamera.targetTexture);
        m_material.SetMatrix(viewProjectionMatrixInvID, mViewProjMatInv);
        m_material.SetFloat(randomSeedID, Random.Range(0.0f, 1.0f));
    }
}
