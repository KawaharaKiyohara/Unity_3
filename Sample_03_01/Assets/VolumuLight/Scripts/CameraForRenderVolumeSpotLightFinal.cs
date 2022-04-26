using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

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
    public class CameraForRenderVolumeSpotLightFinal : MonoBehaviour
    {
        public static CameraForRenderVolumeSpotLightFinal instance { get; private set; }
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
        Material m_material;                    // マテリアル。
        Camera VolumeMapRenderCamera_front;     // ボリュームマップ(前面深度値)を描画するためのカメラ
        Camera VolumeMapRenderCamera_back;      // ボリュームマップ(背面深度値)を描画するためのカメラ。
        ShaderPropertyToID m_shaderPropToId = new ShaderPropertyToID();
        VolumeSpotLightData[] m_volumeSpotLightDataArray = new VolumeSpotLightData[MAX_VOLUME_SPOT_LIGHT];
        GraphicsBuffer m_volumeSpotLightDataGraphicsBuffer;

        private void Awake()
        {
            instance = this;
        }
        void Start()
        {
            m_volumeSpotLightDataGraphicsBuffer = new GraphicsBuffer(
                GraphicsBuffer.Target.Structured,
                MAX_VOLUME_SPOT_LIGHT,
                Marshal.SizeOf(typeof(VolumeSpotLightData))
            );
            var mainCamera = GameObject.Find("Main Camera").GetComponent<Camera>(); ;
            mainCamera.depthTextureMode = DepthTextureMode.Depth;
            m_material = GetComponent<MeshRenderer>().material;
            VolumeMapRenderCamera_front = GameObject.Find("VolumeMapRenderCamera_front").GetComponent<Camera>(); ;
            VolumeMapRenderCamera_back = GameObject.Find("VolumeMapRenderCamera_back").GetComponent<Camera>();
            m_shaderPropToId.volumeFrontTexID = Shader.PropertyToID("volumeFrontTexture");
            m_shaderPropToId.volumeBackTexID = Shader.PropertyToID("volumeBackTexture");
            m_shaderPropToId.viewProjectionMatrixInvID = Shader.PropertyToID("viewProjMatrixInv");
            m_shaderPropToId.randomSeedID = Shader.PropertyToID("ramdomSeed");
            m_shaderPropToId.volumeSpotLightArrayID = Shader.PropertyToID("volumeSpotLightArray");
        }
        public void RegisterVolumeSpotLightData(VolumeSpotLightData data)
        {
            m_volumeSpotLightDataArray[data.no] = data;
        }
        // Update is called once per frame
        void Update()
        {

        }
        private void LateUpdate()
        {

            var projMatrix = GL.GetGPUProjectionMatrix(VolumeMapRenderCamera_front.projectionMatrix, true);
            Matrix4x4 mViewProjMatInv = projMatrix * VolumeMapRenderCamera_front.worldToCameraMatrix;
            mViewProjMatInv = Matrix4x4.Inverse(mViewProjMatInv);
            m_material.SetTexture(m_shaderPropToId.volumeFrontTexID, VolumeMapRenderCamera_front.targetTexture);
            m_material.SetTexture(m_shaderPropToId.volumeBackTexID, VolumeMapRenderCamera_back.targetTexture);
            m_material.SetMatrix(m_shaderPropToId.viewProjectionMatrixInvID, mViewProjMatInv);
            m_material.SetFloat(m_shaderPropToId.randomSeedID, Random.Range(0.0f, 1.0f));

            m_volumeSpotLightDataGraphicsBuffer.SetData(m_volumeSpotLightDataArray);

            m_material.SetBuffer(m_shaderPropToId.volumeSpotLightArrayID, m_volumeSpotLightDataGraphicsBuffer);
        }
    }
}