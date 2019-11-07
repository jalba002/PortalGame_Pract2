using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefractionCube : Companion
{
    public float m_MaxDistance;
    public LayerMask m_CollisionLayerMask;
    public LineRenderer m_LineRenderer;
    private bool m_CreateRefraction;

    void Update()
    {
        m_LineRenderer.gameObject.SetActive(m_CreateRefraction);
        CreateRefraction();
    }

    private void CreateRefraction()
    {
        if (!m_CreateRefraction) return;
        Vector3 l_EndRaycastPosition = Vector3.forward * m_MaxDistance;
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward), out l_RaycastHit, m_MaxDistance, m_CollisionLayerMask.value))
        {
            l_EndRaycastPosition.z = ;
            try
            {
                if (l_RaycastHit.collider.gameObject.GetComponent<RefractionCube>() != null)
                {
                    //Reflect ray
                    l_RaycastHit.collider.GetComponent<RefractionCube>().StartRefracting();
                }
            }
            catch { }
            //Other collisions
        }
        m_LineRenderer.SetPosition(1, l_EndRaycastPosition);
        m_CreateRefraction = false;
    }

    public void StartRefracting()
    {
        m_CreateRefraction = true;
    }
}
