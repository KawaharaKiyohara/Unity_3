using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

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
    public class RenderVolumeSpotLightFinal : MonoBehaviour
    {
        
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
        
        MeshFilter m_meshFilter;                 // ���b�V���t�B���^�[�B
        Material m_material;                    // �}�e���A���B
        
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