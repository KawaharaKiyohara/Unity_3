using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VolumeLight {
    public class RenderVolumeLight : MonoBehaviour
    {
        CommandBuffer m_commandBuffer;
        Camera m_camera;
        VolumeSpotLight[] m_volumeSpotLights;           // �{�����[���X�|�b�g���C�g�B
        RenderTexture m_volumeMapBack;                  // �w�ʂ̐[�x�l���������܂�Ă���{�����[���}�b�v�B
        RenderTexture m_volumeMapFront;                 // �\�ʂ̐[�x�l���������܂�Ă���{�����[���}�b�v�B
        int m_width;                                    // �{�����[���}�b�v�̕��B
        int m_height;                                   // �{�����[���}�b�v�̍����B
        List<MeshRenderer> m_drawBackGameObjects;       // �w�ʂ�`�悷��Q�[���I�u�W�F�N�g�̃��X�g�B
        List<MeshRenderer> m_drawFrontGameObjects;      // �\�ʂ�`�悷��Q�[���I�u�W�F�N�g�̃��X�g�B
        List<MeshFilter> m_drawBackMeshFilter;          // �w�ʂ�`�悷��Q�[���I�u�W�F�N�g�̃��X�g�B
        List<MeshFilter> m_drawFrontMeshFilter;         // �w�ʂ�`�悷��Q�[���I�u�W�F�N�g�̃��X�g�B
        List<RenderVolumeSpotLightFinal> m_drawFinals;
        // Start is called before the first frame update
        void Start()
        {
            m_camera = GetComponent<Camera>();
            m_commandBuffer = new CommandBuffer();
            m_volumeSpotLights = Object.FindObjectsOfType<VolumeSpotLight>();
            m_drawBackGameObjects = new List<MeshRenderer>();
            m_drawFrontGameObjects = new List<MeshRenderer>();
            m_drawBackMeshFilter = new List<MeshFilter>();
            m_drawFrontMeshFilter = new List<MeshFilter>();
            m_drawFinals = new List<RenderVolumeSpotLightFinal>();
            foreach ( var volumeSpotLight in m_volumeSpotLights)
            {
                var trans = volumeSpotLight.transform.Find("VolumeSpotLightBackRenderer");
                var meshRenderer = trans.GetComponent<MeshRenderer>();
                meshRenderer.enabled = false;
                m_drawBackGameObjects.Add(meshRenderer);
                m_drawBackMeshFilter.Add(trans.GetComponent<MeshFilter>());
                trans = volumeSpotLight.transform.Find("VolumeSpotLightFrontRenderer");

                meshRenderer = trans.GetComponent<MeshRenderer>();
                meshRenderer.enabled = false;
                m_drawFrontGameObjects.Add(meshRenderer);

                m_drawFrontMeshFilter.Add(trans.GetComponent<MeshFilter>());
                trans = volumeSpotLight.transform.Find("VolumeSpotLightFinalRenderer");
                m_drawFinals.Add(trans.GetComponent<RenderVolumeSpotLightFinal>());
            }
            // �{�����[���}�b�v�𐶐�
            m_width = Screen.width;
            m_height = Screen.height;
            m_volumeMapBack = new RenderTexture(m_width, m_height, 32, RenderTextureFormat.RGFloat);
            m_volumeMapFront = new RenderTexture(m_volumeMapBack);
        }
        // Update is called once per frame
        void OnPreRender()
        {
            if (m_width != Screen.width || m_height != Screen.height)
            {
                // ��ʉ𑜓x���ς�����̂ō�蒼���B
                m_width = Screen.width;
                m_height = Screen.height;
                m_volumeMapBack = new RenderTexture(m_width, m_height, 32, RenderTextureFormat.RFloat);
                m_volumeMapFront = new RenderTexture(m_volumeMapBack);
            }
            
            for ( int meshNo = 0; meshNo < m_drawBackGameObjects.Count; meshNo++)
            {
                Matrix4x4 mWorld = Matrix4x4.TRS(
                    m_drawBackMeshFilter[meshNo].transform.position,
                    m_drawBackMeshFilter[meshNo].transform.rotation,
                    m_drawBackMeshFilter[meshNo].transform.lossyScale
                );
                // �w�ʂ�`��B
                m_commandBuffer.SetRenderTarget(m_volumeMapBack);
                m_commandBuffer.ClearRenderTarget(true, true, Color.black);
                m_commandBuffer.DrawMesh(
                    m_drawBackMeshFilter[meshNo].mesh,
                    mWorld,
                    m_drawBackGameObjects[meshNo].material
                );

                // �\�ʂ�`��B
                m_commandBuffer.SetRenderTarget(m_volumeMapFront);
                m_commandBuffer.ClearRenderTarget(true, true, Color.black);
                m_commandBuffer.DrawMesh(
                    m_drawFrontMeshFilter[meshNo].mesh,
                    mWorld,
                    m_drawFrontGameObjects[meshNo].material
                );
                m_commandBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);

                m_drawFinals[meshNo].Draw(
                    m_camera,
                    m_volumeMapFront,
                    m_volumeMapBack,
                    m_commandBuffer,
                    m_volumeSpotLights[meshNo].volumeSpotLightData
                );

            }
            m_camera.AddCommandBuffer(CameraEvent.AfterSkybox, m_commandBuffer);
        }
        private void OnPostRender()
        {
            m_camera.RemoveCommandBuffer(CameraEvent.AfterSkybox, m_commandBuffer);
            m_commandBuffer.Clear();
        }
    }
}