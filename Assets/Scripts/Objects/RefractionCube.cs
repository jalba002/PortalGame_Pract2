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
        m_CreateRefraction = false;
    }

    public void CreateRefraction()
    {
        m_CreateRefraction = true;
        Vector3 l_EndRaycastPosition = Vector3.forward * m_MaxDistance;
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward), out l_RaycastHit, m_MaxDistance, m_CollisionLayerMask.value))
        {
            l_EndRaycastPosition = Vector3.forward * l_RaycastHit.distance;
            try
            {
                if (l_RaycastHit.collider.gameObject.GetComponent<RefractionCube>() != null)
                {
                    //Reflect ray
                    l_RaycastHit.collider.GetComponent<RefractionCube>().CreateRefraction();
                }
            }
            catch { }
            //Other collisions
        }
        m_LineRenderer.SetPosition(1, l_EndRaycastPosition);
    }

}
