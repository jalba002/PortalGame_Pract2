using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController : Singleton<GameController>
{
    public class PlayerComponents
    {
        public PlayerHUD m_HUDUpdater;
        public PlayerController m_PlayerController;
        public HealthManager m_PlayerHPMan;
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

    public static bool m_GamePaused = false;
    public static bool m_GameFinished = false;

    public Checkpoint[] m_AreaCheckpoints;

    public Transform m_DestroyInstantiatedObjectsParent;

    [HideInInspector] public List<IRestartable> m_RestartableObjects;

    private void Awake()
    {
        m_PlayerGameObject = FindObjectOfType<PlayerController>().transform.gameObject;
        AddAllRestartableObjects();
        m_PlayerComponents = new PlayerComponents();
        m_PlayerComponents.m_PlayerController = m_PlayerGameObject.GetComponent<PlayerController>();
        m_PlayerComponents.m_PlayerHPMan = m_PlayerGameObject.GetComponent<HealthManager>();
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
        UpdateRestartablesPositions();
    }


    private void Update()
    {
        if (m_GameFinished) return;

        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            bool l_Pause = !m_PlayerCanvasManager.m_PauseMenu.activeSelf;
            m_PlayerCanvasManager.PauseMenu(l_Pause);
            Cursor.lockState = l_Pause ? CursorLockMode.None : CursorLockMode.Locked;
            m_PlayerComponents.m_PlayerController.m_AngleLocked = l_Pause;
            m_GamePaused = l_Pause;
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

    public void PlayerDied()
    {
        //Show menu of playerdied.
        ShowStuff(true);
        m_GameFinished = true;
        m_PlayerCanvasManager.RetryMenu(true);
    }

    public void ShowStuff(bool l_Enable)
    {
        Cursor.lockState = l_Enable ? CursorLockMode.None : CursorLockMode.Locked;
        m_PlayerComponents.m_PlayerController.m_AngleLocked = l_Enable;
        m_GamePaused = l_Enable;
    }

    public void RetryGame()
    {
        CheckPointManager.SetNewCheckpoint(m_AreaCheckpoints[0]);
        RestartAllObjects();

        HideMenusAndControls();
    }

    public void HideMenusAndControls()
    {
        StartCoroutine(BecomeVulnerable());
        ShowStuff(false);

        m_PlayerCanvasManager.RetryMenu(false);
        m_PlayerCanvasManager.PauseMenu(false);
        m_PlayerCanvasManager.WinScreen(false);

        m_GameFinished = false;
    }

    public void RestartGame()
    {
        RestartAllObjects();

        HideMenusAndControls();
    }

    public void Win()
    {
        ShowStuff(true);
        m_GameFinished = true;
        m_PlayerCanvasManager.WinScreen(true);
    }

    public IEnumerator BecomeVulnerable()
    {
        m_PlayerComponents.m_PlayerHPMan.m_Invulnerable = true;
        yield return null;
        m_PlayerComponents.m_PlayerHPMan.m_Invulnerable = false;
    }

    public void UpdateRestartablesPositions()
    {
        foreach (IRestartable l_Object in m_RestartableObjects)
        {
            l_Object.UpdateValues();
        }
    }
}
