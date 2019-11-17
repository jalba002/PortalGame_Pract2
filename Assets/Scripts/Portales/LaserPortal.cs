using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserPortal : MonoBehaviour
{
    Portal m_AttachedPortal;
    [Header("Laser Settings")]
    public float m_MaxDistance;
    public LayerMask m_CollisionLayerMask;
    public LineRenderer m_LineRenderer;
    private bool m_CreateRefraction;
    private bool m_CubeRefracted;

    private void Start()
    {
        m_AttachedPortal = GetComponentInParent<Portal>();
        m_CubeRefracted = false;
    }

    private void Update()
    {
        m_AttachedPortal.m_MirrorPortal.m_Laser.m_LineRenderer.enabled = m_CubeRefracted;
        m_CubeRefracted = false;
    }

    public void SetCollisionPosition(Vector3 l_CollisionPoint, Vector3 l_Direction)
    {
        if (m_CubeRefracted) return;

        m_CubeRefracted = true;
        m_AttachedPortal.m_MirrorPortal.m_Laser.m_LineRenderer.enabled = m_CubeRefracted;

        Vector3 l_LocalPosition = this.gameObject.transform.InverseTransformPoint(l_CollisionPoint);
        l_LocalPosition = new Vector3(-l_LocalPosition.x, l_LocalPosition.y, -l_LocalPosition.z);

        Vector3 l_LocalDirection = this.gameObject.transform.InverseTransformDirection(l_Direction);
        l_LocalDirection = new Vector3(-l_LocalDirection.x, l_LocalDirection.y, -l_LocalDirection.z);

        Vector3 l_EndRayCastPosition = this.gameObject.transform.InverseTransformPoint((l_CollisionPoint + l_LocalDirection * m_MaxDistance));

        m_AttachedPortal.m_MirrorPortal.m_Laser.m_LineRenderer.SetPosition(0, l_LocalPosition);

        Vector3 l_RayLocalPosition = m_AttachedPortal.m_MirrorPortal.transform.TransformPoint(l_LocalPosition);

        RaycastHit l_RaycastHit;
        Debug.DrawRay(l_RayLocalPosition, -l_Direction * 5f, Color.green);
        if (Physics.Raycast(new Ray(l_RayLocalPosition, -l_Direction), out l_RaycastHit, m_MaxDistance, m_CollisionLayerMask.value))
        {
            l_EndRayCastPosition = l_RaycastHit.point;
            try
            {
                if (l_RaycastHit.collider.gameObject.GetComponent<RefractionCube>() != null)
                {
                    //Reflect ray
                    l_RaycastHit.collider.GetComponent<RefractionCube>().CreateRefraction();
                }
                else if (l_RaycastHit.collider.gameObject.GetComponent<Turret>() != null)
                {
                    //DESTROY
                }
            }
            catch { }
            l_EndRayCastPosition = m_AttachedPortal.m_MirrorPortal.transform.InverseTransformPoint(l_EndRayCastPosition);
        }
        m_AttachedPortal.m_MirrorPortal.m_Laser.m_LineRenderer.SetPosition(1, l_EndRayCastPosition);

    }

}
