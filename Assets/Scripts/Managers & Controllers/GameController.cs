using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController : Singleton<GameController>
{
    public class PlayerComponents
    {
        public PlayerHUD m_HUDUpdater;
    }
    public PlayerComponents m_PlayerComponents;
    private GameObject m_PlayerGameObject;
    public CanvasManager m_PlayerCanvasManager;
    public Transform m_PortalCheckerPoint;
    public List<Transform> m_PortalCheckersList = new List<Transform>();
    public Portal m_BluePortal;
    public Portal m_OrangePortal;
    public GameObject m_PortalPreview;
    public GameObject m_RedPortalPreview;

    public Transform m_DestroyInstantiatedObjectsParent;

    [HideInInspector] public List<IRestartable> m_RestartableObjects;

    private void Awake()
    {
        m_PlayerGameObject = FindObjectOfType<PlayerController>().transform.gameObject;
    }

    void Start()
    {
        AddAllRestartableObjects();
        m_PlayerComponents = new PlayerComponents();
        foreach (Transform T in m_PortalCheckerPoint.GetComponentsInChildren<Transform>())
        {
            m_PortalCheckersList.Add(T);
        }
    }

    void AddAllRestartableObjects()
    {
        m_RestartableObjects = new List<IRestartable>();
        var ss = FindObjectsOfType<MonoBehaviour>().OfType<IRestartable>();
        foreach (IRestartable res in ss)
        {
            m_RestartableObjects.Add(res);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            RestartAllObjects();
        }
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

    public void RestartAllObjects()
    {
        foreach (IRestartable l_Object in m_RestartableObjects)
        {
            l_Object.Restart();
        }
        try
        {
            Companion[] l_DestroyObjects = m_DestroyInstantiatedObjectsParent.GetComponentsInChildren<Companion>();
            foreach (Companion T in l_DestroyObjects)
            {
                Destroy(T.gameObject);
            }
            l_DestroyObjects = null;
        }
        catch { }
    }
}
