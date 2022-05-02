using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

namespace VolumeLight
{
    /// <summary>
    /// ボリュームスポットライトのデータ構造体。
    /// </summary>
    public struct VolumeSpotLightData
    {
        public Vector3 position;           // 座標
        public int isUse;                  // 使用中フラグ。
        public Vector3 positionInView;     // カメラ空間での座標。
        public int no;                     // ライトの番号。
        public Vector3 direction;          // 射出方向。
        public Vector3 range;                // 影響範囲。
        public Vector3 color;              // ライトのカラー。
        public Vector3 color2;             // 二つ目のカラー。
        public Vector3 color3;             // 三つ目のカラー。
        public Vector3 directionInView;    // カメラ空間での射出方向。
        public Vector3 rangePow;           // 距離による光の影響率に累乗するパラメーター。1.0で線形の変化をする。
                                           // xが一つ目のカラー、yが二つ目のカラー、zが三つ目のカラー。
        public Vector3 angle;              // 射出角度(単位：ラジアン。xが一つ目のカラー、yが二つ目のカラー、zが三つ目のカラー)。
        public Vector3 anglePow;           // スポットライトとの角度による光の影響率に累乗するパラメータ。1.0で線形に変化する。
                                           // xが一つ目のカラー、yが二つ目のカラー、zが三つ目のカラー。
    };
    public class RenderVolumeSpotLightFinal : MonoBehaviour
    {
        
        /// <summary>
        /// シェーダープロパティIDをまとめた構造体
        /// </summary>
        struct ShaderPropertyToID
        {
            public int volumeFrontTexID;
            public int volumeBackTexID;
            public int viewProjectionMatrixInvID;
            public int randomSeedID;
            public int volumeSpotLightArrayID;
        };


        const int MAX_VOLUME_SPOT_LIGHT = 1000; // ボリュームスポとライトの最大数。
        
        MeshFilter m_meshFilter;                 // メッシュフィルター。
        Material m_material;                    // マテリアル。
        
        ShaderPropertyToID m_shaderPropToId = new ShaderPropertyToID();
        VolumeSpotLightData[] m_volumeSpotLightDataArray = new VolumeSpotLightData[1];
        GraphicsBuffer m_volumeSpotLightDataGraphicsBuffer;
        
        
        void Start()
        {
            m_volumeSpotLightDataGraphicsBuffer = new GraphicsBuffer(
                GraphicsBuffer.Target.Structured,
                1,
                Marshal.SizeOf(typeof(VolumeSpotLightData))
            );
            var meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.enabled = false;
            m_meshFilter = GetComponent<MeshFilter>();
            m_material = meshRenderer.material;
            
            m_shaderPropToId.volumeFrontTexID = Shader.PropertyToID("volumeFrontTexture");
            m_shaderPropToId.volumeBackTexID = Shader.PropertyToID("volumeBackTexture");
            m_shaderPropToId.viewProjectionMatrixInvID = Shader.PropertyToID("viewProjMatrixInv");
            m_shaderPropToId.randomSeedID = Shader.PropertyToID("ramdomSeed");
            m_shaderPropToId.volumeSpotLightArrayID = Shader.PropertyToID("volumeSpotLightArray");

            
        }
        
        public void Draw(Camera camera, RenderTexture volumeMapFront, RenderTexture VolumeMapBack, CommandBuffer commandBuffer, VolumeSpotLightData data)
        {
            m_volumeSpotLightDataArray[0] = data;
            var projMatrix = GL.GetGPUProjectionMatrix(camera.projectionMatrix, true);
            Matrix4x4 mViewProjMatInv = projMatrix * camera.worldToCameraMatrix;
            mViewProjMatInv = Matrix4x4.Inverse(mViewProjMatInv);
            Matrix4x4 m = Matrix4x4.TRS(Vector3.zero, Quaternion.identity, transform.lossyScale);

            m_material.SetTexture(m_shaderPropToId.volumeFrontTexID, volumeMapFront);
            m_material.SetTexture(m_shaderPropToId.volumeBackTexID, VolumeMapBack);
            m_material.SetMatrix(m_shaderPropToId.viewProjectionMatrixInvID, mViewProjMatInv);
            m_material.SetFloat(m_shaderPropToId.randomSeedID, Random.Range(0.0f, 1.0f));
            
            m_volumeSpotLightDataGraphicsBuffer.SetData(m_volumeSpotLightDataArray);

            m_material.SetBuffer(m_shaderPropToId.volumeSpotLightArrayID, m_volumeSpotLightDataGraphicsBuffer);

            commandBuffer.DrawMesh(
                 m_meshFilter.mesh,
                 m,
                 m_material
             );
        }
    }
}