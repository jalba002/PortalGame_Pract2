using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : MonoBehaviour
{
    public LineRenderer m_LineRenderer;
    public LayerMask m_CollisionLayerMask;
    public float m_MaxDistance;
    public float m_AngleLaserActive;

    private float l_DotAngleLaserActive;
    private bool rayActive;
    public bool l_RayActive
    {
        get
        {
            return rayActive;
        }
        set
        {
            rayActive = value;
            m_LineRenderer.gameObject.SetActive(value);
        }
    }

    public void AdaptSize()
    {
        Vector3 l_SelfScale = m_LineRenderer.transform.localScale;
        l_SelfScale.z = 1f - (transform.localScale.z - 1.05f);
        m_LineRenderer.transform.localScale = l_SelfScale;
    }

    void Start()
    {
        l_DotAngleLaserActive = Mathf.Cos(m_AngleLaserActive * Mathf.Deg2Rad * 0.5f);
        l_RayActive = Vector3.Dot(transform.up, Vector3.up) > l_DotAngleLaserActive;
    }

    void Update()
    {
        l_RayActive = Vector3.Dot(transform.up, Vector3.up) > l_DotAngleLaserActive;
        CastLaser();
    }

    private void CastLaser()
    {
        if (!l_RayActive)
        {
            return;
        }

        Vector3 l_EndRaycastPosition = Vector3.forward * m_MaxDistance;
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward), out l_RaycastHit, m_MaxDistance, m_CollisionLayerMask.value))
        {
            l_EndRaycastPosition = Vector3.forward * l_RaycastHit.distance;
            try
            {
                if (l_RaycastHit.collider.gameObject.GetComponent<RefractionCube>() != null)
                {
                    l_RaycastHit.collider.GetComponent<RefractionCube>().CreateRefraction();
                }
                else if(l_RaycastHit.collider.gameObject.GetComponent<LaserPortal>() != null)
                {
                    l_RaycastHit.collider.GetComponent<LaserPortal>().Collide(l_RaycastHit.point, this.gameObject.transform.forward);
                }
            }
            catch { }
        }
        m_LineRenderer.SetPosition(1, l_EndRaycastPosition);
    }
}
