using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public class PlayerComponents
    {
        public PlayerHUD m_HUDUpdater;
    }
    public PlayerComponents m_PlayerComponents;
    private GameObject m_PlayerGameObject;

    public Transform m_PortalCheckerPoint;
    public List<Transform> m_PortalCheckersList = new List<Transform>();
    public Portal m_BluePortal;
    public Portal m_OrangePortal;

    private void Awake()
    {
        m_PlayerGameObject = FindObjectOfType<PlayerController>().transform.gameObject;
    }

    void Start()
    {
        m_PlayerComponents = new PlayerComponents();
        foreach (Transform T in m_PortalCheckerPoint.GetComponentsInChildren<Transform>())
        {
            m_PortalCheckersList.Add(T);
        }
    }

    void Update()
    {

    }

    public GameObject GetPlayerGameObject()
    {
        return m_PlayerGameObject;
    }

    public void ChangeLayer(GameObject l_Object, bool l_Ignore)
    {
        if (l_Object == m_PlayerGameObject)
            l_Object.layer = l_Ignore ? 11 : 10;
        else
            l_Object.layer = l_Ignore ? 16 : 15;
    }
}
