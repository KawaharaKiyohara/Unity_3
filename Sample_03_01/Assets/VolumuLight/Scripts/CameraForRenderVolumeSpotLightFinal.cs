using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace VolumeLight
{
    /// <summary>
    /// �{�����[���X�|�b�g���C�g�̃f�[�^�\���́B
    /// </summary>
    public struct VolumeSpotLightData
    {
        public Vector3 position;           // ���W
        public int isUse;                  // �g�p���t���O�B
        public Vector3 positionInView;     // �J������Ԃł̍��W�B
        public int no;                     // ���C�g�̔ԍ��B
        public Vector3 direction;          // �ˏo�����B
        public Vector3 range;                // �e���͈́B
        public Vector3 color;              // ���C�g�̃J���[�B
        public Vector3 color2;             // ��ڂ̃J���[�B
        public Vector3 color3;             // �O�ڂ̃J���[�B
        public Vector3 directionInView;    // �J������Ԃł̎ˏo�����B
        public Vector3 rangePow;           // �����ɂ����̉e�����ɗݏ悷��p�����[�^�[�B1.0�Ő��`�̕ω�������B
                                           // x����ڂ̃J���[�Ay����ڂ̃J���[�Az���O�ڂ̃J���[�B
        public Vector3 angle;              // �ˏo�p�x(�P�ʁF���W�A���Bx����ڂ̃J���[�Ay����ڂ̃J���[�Az���O�ڂ̃J���[)�B
        public Vector3 anglePow;           // �X�|�b�g���C�g�Ƃ̊p�x�ɂ����̉e�����ɗݏ悷��p�����[�^�B1.0�Ő��`�ɕω�����B
                                           // x����ڂ̃J���[�Ay����ڂ̃J���[�Az���O�ڂ̃J���[�B
    };
    public class CameraForRenderVolumeSpotLightFinal : MonoBehaviour
    {
        public static CameraForRenderVolumeSpotLightFinal instance { get; private set; }
        /// <summary>
        /// �V�F�[�_�[�v���p�e�BID���܂Ƃ߂��\����
        /// </summary>
        struct ShaderPropertyToID
        {
            public int volumeFrontTexID;
            public int volumeBackTexID;
            public int viewProjectionMatrixInvID;
            public int randomSeedID;
            public int volumeSpotLightArrayID;
        };


        const int MAX_VOLUME_SPOT_LIGHT = 1000; // �{�����[���X�|�ƃ��C�g�̍ő吔�B
        Material m_material;                    // �}�e���A���B
        Camera VolumeMapRenderCamera_front;     // �{�����[���}�b�v(�O�ʐ[�x�l)��`�悷�邽�߂̃J����
        Camera VolumeMapRenderCamera_back;      // �{�����[���}�b�v(�w�ʐ[�x�l)��`�悷�邽�߂̃J�����B
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