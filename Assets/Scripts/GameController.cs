using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
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
        m_PlayerGameObject.layer = l_Ignore ? 11 : 10;
    }
}
