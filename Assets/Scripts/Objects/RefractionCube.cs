using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefractionCube : Companion
{
    public float m_MaxDistance;
    public LayerMask m_CollisionLayerMask;
    public LineRenderer m_LineRenderer;
    private bool m_CreateRefraction;
    private bool m_CubeRefracted;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        m_OnSizeChange.AddListener(AdaptSize);
        base.Start();
    }

    private void AdaptSize()
    {
        Vector3 l_SelfScale = m_LineRenderer.transform.localScale;
        l_SelfScale.z = transform.localScale.z * 3f;
        m_LineRenderer.transform.localScale = l_SelfScale;
    }

    void Update()
    {
        m_LineRenderer.gameObject.SetActive(m_CreateRefraction);
        m_CreateRefraction = false;
        m_CubeRefracted = false;
    }

    public void CreateRefraction()
    {
        //if (m_CreateRefraction) return;
        if (m_CubeRefracted) return;

        m_CubeRefracted = true;
        m_CreateRefraction = true;
        m_LineRenderer.gameObject.SetActive(m_CreateRefraction);

        Vector3 l_EndRaycastPosition = Vector3.forward * m_MaxDistance;
        RaycastHit l_RaycastHit;

        if (Physics.Raycast(new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward), out l_RaycastHit, m_MaxDistance, m_CollisionLayerMask.value))
        {
            l_EndRaycastPosition = Vector3.forward * l_RaycastHit.distance * 1.05f;
            try
            {
                if (l_RaycastHit.collider.gameObject.GetComponent<RefractionCube>() != null)
                {
                    //Reflect ray
                    l_RaycastHit.collider.GetComponent<RefractionCube>().CreateRefraction();
                }
                else if (l_RaycastHit.collider.gameObject.GetComponent<LaserPortal>() != null)
                {
                    l_RaycastHit.collider.GetComponent<LaserPortal>().Collide(l_RaycastHit.point, this.gameObject.transform.forward);
                }
            }
            catch { }
            //Other collisions
        }

        m_LineRenderer.SetPosition(1, l_EndRaycastPosition);
    }
}
