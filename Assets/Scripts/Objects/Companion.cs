using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Companion : MonoBehaviour , IPickable
{
    private Vector3 currentScale;
    public Vector3 m_CurrentScale
    {
        get
        {
            return currentScale;
        }
        set
        {
            Vector3 newValue = value;
            newValue = Vector3.Max(newValue, m_MinScale);
            newValue = Vector3.Min(newValue, m_MaxScale);
            currentScale = newValue;
            this.transform.localScale = currentScale;
        }
    }

    public Vector3 m_MaxScale;
    public Vector3 m_MinScale;
    public bool m_PickedUp;
  
    private Vector3 m_OriginalScale;
    private bool m_Teleportable;

    void Start()
    {
        m_Teleportable = true;
        m_OriginalScale = this.transform.localScale;
        m_MinScale = m_OriginalScale * 0.5f;
        m_MaxScale = m_OriginalScale * 2f;
        m_CurrentScale = m_OriginalScale;
    }

    public void SetTeleport(bool l_Activate)
    {
        m_Teleportable = l_Activate;
    }

    public void SetNewScale(Vector3 l_NewScale)
    {
        m_CurrentScale = l_NewScale;
    }

    public bool IsTeleportable()
    {
        return m_Teleportable;
    }

}
