using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public LayerMask m_IgnoreLayersOnPortalCollision;
    public LayerMask m_PlayerLayer;

    private GameObject m_PlayerGameObject;

    private void Awake()
    {
        m_PlayerGameObject = FindObjectOfType<PlayerController>().transform.gameObject;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public GameObject GetPlayerGameObject()
    {
        return m_PlayerGameObject;
    }

    public void PlayerIgnoreLayers(bool l_Ignore)
    {
        int mPlayerLayerShitfted = m_PlayerLayer.value >> 4;
        Debug.Log(mPlayerLayerShitfted);
        Physics.IgnoreLayerCollision(m_PlayerLayer.value >> 8, m_IgnoreLayersOnPortalCollision.value >> 8, l_Ignore);
    }
}
