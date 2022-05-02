using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VolumeLight {
    public class RenderVolumeLight : MonoBehaviour
    {
        CommandBuffer m_commandBuffer;
        Camera m_camera;
        VolumeSpotLight[] m_volumeSpotLights;           // ボリュームスポットライト。
        RenderTexture m_volumeMapBack;                  // 背面の深度値が書き込まれているボリュームマップ。
        RenderTexture m_volumeMapFront;                 // 表面の深度値が書き込まれているボリュームマップ。
        int m_width;                                    // ボリュームマップの幅。
        int m_height;                                   // ボリュームマップの高さ。
        List<MeshRenderer> m_drawBackGameObjects;       // 背面を描画するゲームオブジェクトのリスト。
        List<MeshRenderer> m_drawFrontGameObjects;      // 表面を描画するゲームオブジェクトのリスト。
        List<MeshFilter> m_drawBackMeshFilter;          // 背面を描画するゲームオブジェクトのリスト。
        List<MeshFilter> m_drawFrontMeshFilter;         // 背面を描画するゲームオブジェクトのリスト。
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
            // ボリュームマップを生成
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
                // 画面解像度が変わったので作り直し。
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
                // 背面を描画。
                m_commandBuffer.SetRenderTarget(m_volumeMapBack);
                m_commandBuffer.ClearRenderTarget(true, true, Color.black);
                m_commandBuffer.DrawMesh(
                    m_drawBackMeshFilter[meshNo].mesh,
                    mWorld,
                    m_drawBackGameObjects[meshNo].material
                );

                // 表面を描画。
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