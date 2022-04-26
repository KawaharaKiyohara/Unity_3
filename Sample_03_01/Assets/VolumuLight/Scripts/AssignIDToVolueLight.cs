using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeLight
{
    /// <summary>
    /// �{�����[�����C�g��ID�����蓖�Ă�
    /// </summary>
    public class AssignIDToVolueLight : MonoBehaviour
    {
        public static AssignIDToVolueLight instance { get; private set; }
        int m_assigneId = 0;    // �A�T�C�������ID
        private void Awake()
        {
            instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {

        }
        /// <summary>
        /// �{�����[�����C�g��ID���A�T�C������B
        /// </summary>
        /// <returns></returns>
        public void AssignIDToVolumeLight(VolumeSpotLight volumeSpotLight)
        {
            volumeSpotLight.id = m_assigneId;
            m_assigneId++;
        }
        private void LateUpdate()
        {
            // �A�T�C������ID�����Z�b�g�B
            m_assigneId = 0;
        }
    }
}