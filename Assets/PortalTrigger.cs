using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PortalTrigger : MonoBehaviour
{
    GameObject m_PlayerGameObject;

    private bool m_PlayerInsideCollider;
    private void Start()
    {
        m_PlayerInsideCollider = false;
        m_PlayerGameObject = GameController.Instance.GetPlayerGameObject();
    }

    private void Update()
    {
        if (!m_PlayerInsideCollider) return;
        

        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == m_PlayerGameObject)
        {
            m_PlayerInsideCollider = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == m_PlayerGameObject)
        {
            m_PlayerInsideCollider = false;
        }
    }
}
