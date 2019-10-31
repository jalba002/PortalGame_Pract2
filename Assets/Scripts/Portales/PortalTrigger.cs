using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PortalTrigger : MonoBehaviour
{
    private Portal m_AttachedPortal;
    private GameObject m_PlayerGameObject;

    private void Start()
    {
        m_AttachedPortal = GetComponentInParent<Portal>();
        m_PlayerGameObject = GameController.Instance.GetPlayerGameObject();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == m_PlayerGameObject)
        {
            m_AttachedPortal.PlayerInsideCollider(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == m_PlayerGameObject)
        {
            m_AttachedPortal.PlayerInsideCollider(false);
        }
    }
}
