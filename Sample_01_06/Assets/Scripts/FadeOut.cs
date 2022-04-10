using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    List<Material> m_materialList = new List<Material>();
    float m_clipParam = 0.0f;
    float m_clipVel = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        SkinnedMeshRenderer[] renderList = transform.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var render in renderList)
        {
            m_materialList.Add(render.material);
        }
    }
    // Update is called once per frame
    void Update()
    {
        m_clipParam = Mathf.SmoothDamp(m_clipParam, 1.1f, ref m_clipVel, 1.0f );
        foreach (var mat in m_materialList)
        {
            mat.SetFloat("_clipParam", m_clipParam);
        }
    }
}
