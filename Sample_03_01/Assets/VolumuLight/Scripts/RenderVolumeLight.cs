using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VolumeLight {
    /// <summary>
    /// ボリュームライト描画処理。
    /// </summary>
    public class RenderVolumeLight : MonoBehaviour
    {
        CommandBuffer m_commandBuffer;                  // コマンドバッファ。
        Camera m_camera;                                // カメラ。
        VolumeSpotLight[] m_volumeSpotLights;           // ボリュームスポットライト。
        RenderTexture m_volumeMapBack;                  // 背面の深度値が書き込まれているボリュームマップ。
        RenderTexture m_volumeMapFront;                 // 表面の深度値が書き込まれているボリュームマップ。
        int m_volumeMapWidth;                           // ボリュームマップの幅。
        int m_volumeMapHeight;                          // ボリュームマップの高さ。
        List<Material> m_drawBackMaterialList;          // 背面の深度値描画で使用するマテリアルのリスト。
        List<Material> m_drawFrontMaterialList;         // 表面の深度値描画で使用するマテリアルのリスト。
        List<MeshFilter> m_drawBackMeshFilterList;      // 背面の深度値描画で使用するメッシュフィルターのリスト。
        List<MeshFilter> m_drawFrontMeshFilterList;     // 表面の深度値描画で使用するメッシュフィルターのリスト。
        List<RenderVolumeSpotLightFinal> m_drawFinals;

        // Start is called before the first frame update
        void Start()
        {
            m_camera = GetComponent<Camera>();
            m_commandBuffer = new CommandBuffer();
            m_volumeSpotLights = Object.FindObjectsOfType<VolumeSpotLight>();
            m_drawBackMaterialList = new List<Material>();
            m_drawFrontMaterialList = new List<Material>();
            m_drawBackMeshFilterList = new List<MeshFilter>();
            m_drawFrontMeshFilterList = new List<MeshFilter>();
            m_drawFinals = new List<RenderVolumeSpotLightFinal>();
            foreach ( var volumeSpotLight in m_volumeSpotLights)
            {
                var trans = volumeSpotLight.transform.Find("BackRenderer");
                var meshRenderer = trans.GetComponent<MeshRenderer>();
                meshRenderer.enabled = false;
                m_drawBackMaterialList.Add(meshRenderer.material);
                m_drawBackMeshFilterList.Add(trans.GetComponent<MeshFilter>());

                trans = volumeSpotLight.transform.Find("FrontRenderer");
                meshRenderer = trans.GetComponent<MeshRenderer>();
                meshRenderer.enabled = false;
                m_drawFrontMaterialList.Add(meshRenderer.material);
                m_drawFrontMeshFilterList.Add(trans.GetComponent<MeshFilter>());

                trans = volumeSpotLight.transform.Find("FinalRenderer");
                m_drawFinals.Add(trans.GetComponent<RenderVolumeSpotLightFinal>());
            }
            // ボリュームマップを生成
            m_volumeMapWidth = Screen.width;
            m_volumeMapHeight = Screen.height;
            m_volumeMapBack = new RenderTexture(m_volumeMapWidth, m_volumeMapHeight, 32, RenderTextureFormat.RGFloat);
            m_volumeMapFront = new RenderTexture(m_volumeMapBack);
        }
        // Update is called once per frame
        void OnPreRender()
        {
            if (m_volumeMapWidth != Screen.width || m_volumeMapHeight != Screen.height)
            {
                // 画面解像度が変わったので作り直し。
                m_volumeMapWidth = Screen.width;
                m_volumeMapHeight = Screen.height;
                m_volumeMapBack = new RenderTexture(m_volumeMapWidth, m_volumeMapHeight, 32, RenderTextureFormat.RFloat);
                m_volumeMapFront = new RenderTexture(m_volumeMapBack);
            }
            
            for ( int litNo = 0; litNo < m_drawBackMaterialList.Count; litNo++)
            {
                Matrix4x4 mWorld = Matrix4x4.TRS(
                    m_drawBackMeshFilterList[litNo].transform.position,
                    m_drawBackMeshFilterList[litNo].transform.rotation,
                    m_drawBackMeshFilterList[litNo].transform.lossyScale
                );
                // 背面を描画。
                m_commandBuffer.SetRenderTarget(m_volumeMapBack);
                m_commandBuffer.ClearRenderTarget(true, true, Color.black);
                m_commandBuffer.DrawMesh(
                    m_drawBackMeshFilterList[litNo].mesh,
                    mWorld,
                    m_drawBackMaterialList[litNo]
                );

                // 表面を描画。
                m_commandBuffer.SetRenderTarget(m_volumeMapFront);
                m_commandBuffer.ClearRenderTarget(true, true, Color.black);
                m_commandBuffer.DrawMesh(
                    m_drawFrontMeshFilterList[litNo].mesh,
                    mWorld,
                    m_drawFrontMaterialList[litNo]
                );
                m_commandBuffer.SetRenderTarget(BuiltinRenderTextureType.CameraTarget);

                m_drawFinals[litNo].Draw(
                    m_camera,
                    m_volumeMapFront,
                    m_volumeMapBack,
                    m_commandBuffer,
                    m_volumeSpotLights[litNo].volumeSpotLightData
                );

            }
            m_camera.AddCommandBuffer(CameraEvent.BeforeForwardAlpha, m_commandBuffer);
        }
        private void OnPostRender()
        {
            m_camera.RemoveCommandBuffer(CameraEvent.BeforeForwardAlpha, m_commandBuffer);
            m_commandBuffer.Clear();
        }
    }
}