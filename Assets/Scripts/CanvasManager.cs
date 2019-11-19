using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public Sprite[] m_HUDImages = new Sprite[4];
    public Image m_Crosshair;

    public GameObject m_PauseMenu;
    public GameObject m_RetryMenu;
    public GameObject m_WinScreen;

    public void UpdateHud()
    {
        if (GameController.Instance.m_OrangePortal.gameObject.activeSelf && GameController.Instance.m_BluePortal.gameObject.activeSelf)
        {
            m_Crosshair.sprite = m_HUDImages[0];
        }
        else if (!GameController.Instance.m_OrangePortal.gameObject.activeSelf && GameController.Instance.m_BluePortal.gameObject.activeSelf)
        {
            m_Crosshair.sprite = m_HUDImages[1];
        }
        else if (!GameController.Instance.m_BluePortal.gameObject.activeSelf && GameController.Instance.m_OrangePortal.gameObject.activeSelf)
        {
            m_Crosshair.sprite = m_HUDImages[2];
        }
        else
        {
            m_Crosshair.sprite = m_HUDImages[3];
        }
    }

    public void PauseMenu(bool l_Enable)
    {
        Time.timeScale = l_Enable ? 0.0f : 1.0f;
        m_PauseMenu.SetActive(l_Enable);
    }

    public void RetryMenu(bool l_Enable)
    {
        Time.timeScale = l_Enable ? 0.0f : 1.0f;
        m_RetryMenu.SetActive(l_Enable);
    }

    public void WinScreen(bool l_Enable)
    {
        Time.timeScale = l_Enable ? 0.0f : 1.0f;
        m_WinScreen.SetActive(l_Enable);
    }
}
