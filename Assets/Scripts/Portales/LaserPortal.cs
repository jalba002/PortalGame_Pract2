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

    private ButtonInteractable m_LastButtonHit = null;
    private bool m_CreateRefraction;
    private bool m_CubeRefracted;

    private void Start()
    {
        m_AttachedPortal = GetComponentInParent<Portal>();
        m_CubeRefracted = false;
    }

    private void Update()
    {
        m_LineRenderer.enabled = m_CubeRefracted;
        if (m_CubeRefracted)
        {
            m_CubeRefracted = false;
            if(!m_CubeRefracted && m_LastButtonHit != null && m_CreateRefraction)
            {
                m_LastButtonHit.ForceStop();
                m_LastButtonHit = null;
            }
            m_CreateRefraction = false;
        }
    }

    public void Collide(Vector3 l_CollisionPoint, Vector3 l_Direction)
    {
        m_AttachedPortal.m_MirrorPortal.m_Laser.CreateRefraction(l_CollisionPoint, l_Direction);
    }

    public void CreateRefraction(Vector3 l_Position, Vector3 l_Direction) //This is a mess and took me 4+ hours.
    {
        if (m_CubeRefracted) return;

        this.m_CreateRefraction = true;
        this.m_CubeRefracted = true;
        this.m_LineRenderer.enabled = true;

        #region Vector3 Declarations
        Vector3 l_EndRayCastPosition;
        Vector3 l_CollisionPointOnLocal;
        Vector3 l_CollisionPointOnWorld;
        Vector3 l_DirectionOnWorld;
        #endregion

        l_CollisionPointOnLocal = m_AttachedPortal.m_MirrorPortal.transform.InverseTransformPoint(l_Position);
        l_CollisionPointOnLocal = new Vector3(-l_CollisionPointOnLocal.x, l_CollisionPointOnLocal.y, -l_CollisionPointOnLocal.z);

        m_LineRenderer.SetPosition(0, l_CollisionPointOnLocal);

        //Setting the starting position!
        l_EndRayCastPosition = l_CollisionPointOnLocal + l_Direction * m_MaxDistance;

        //Changing a lot!
        l_CollisionPointOnWorld = m_AttachedPortal.transform.TransformPoint(l_CollisionPointOnLocal);
        l_DirectionOnWorld = m_AttachedPortal.m_MirrorPortal.transform.InverseTransformDirection(l_Direction);
        l_DirectionOnWorld = this.m_AttachedPortal.transform.TransformDirection(l_DirectionOnWorld);
        l_DirectionOnWorld = new Vector3(-l_DirectionOnWorld.x, l_DirectionOnWorld.y, -l_DirectionOnWorld.z);
        //Moving from Local to World! For the raycast!

        //Time for a Raycast!
        Ray l_WorldRay = new Ray(l_CollisionPointOnWorld + (l_DirectionOnWorld * 0.1f), l_DirectionOnWorld);
        RaycastHit l_RayCastHit;
        if (Physics.Raycast(l_WorldRay, out l_RayCastHit, m_MaxDistance, m_CollisionLayerMask))
        {
            l_EndRayCastPosition = this.gameObject.transform.InverseTransformPoint(l_RayCastHit.point); //The hit in raycast is WORLD, we need it in LOCAL for the LineRenderer!
            try
            {
                if (l_RayCastHit.collider.gameObject.GetComponent<RefractionCube>() != null)
                {
                    l_RayCastHit.collider.gameObject.GetComponent<RefractionCube>().CreateRefraction();
                }
                if (l_RayCastHit.collider.gameObject.GetComponent<ButtonInteractable>() != null)
                {
                    m_LastButtonHit = l_RayCastHit.collider.gameObject.GetComponent<ButtonInteractable>();
                    m_LastButtonHit.Interact();
                }
                else if (m_LastButtonHit != null)
                {
                    m_LastButtonHit.ForceStop();
                    m_LastButtonHit = null;
                }
            }
            catch
            {
                //Fail! But it has to!
            }
        }
        //End raycast!
        m_LineRenderer.SetPosition(1, l_EndRayCastPosition);
    }

}
