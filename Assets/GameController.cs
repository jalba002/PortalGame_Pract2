using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public LayerMask m_IgnoreLayersOnPortalCollision;

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
}
