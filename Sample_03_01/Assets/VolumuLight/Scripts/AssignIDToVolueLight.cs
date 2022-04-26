using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VolumeLight
{
    /// <summary>
    /// ボリュームライトにIDを割り当てる
    /// </summary>
    public class AssignIDToVolueLight : MonoBehaviour
    {
        public static AssignIDToVolueLight instance { get; private set; }
        int m_assigneId = 0;    // アサインされるID
        private void Awake()
        {
            instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {

        }
        /// <summary>
        /// ボリュームライトにIDをアサインする。
        /// </summary>
        /// <returns></returns>
        public void AssignIDToVolumeLight(VolumeSpotLight volumeSpotLight)
        {
            volumeSpotLight.id = m_assigneId;
            m_assigneId++;
        }
        private void LateUpdate()
        {
            // アサインするIDをリセット。
            m_assigneId = 0;
        }
    }
}