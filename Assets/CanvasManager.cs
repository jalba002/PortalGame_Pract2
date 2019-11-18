using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public Sprite[] m_HUDImages = new Sprite[4];
    public Image m_Crosshair;

    public void UpdateHud()
    {
        Debug.Log("Updating HUD");
        if (GameController.Instance.m_OrangePortal.gameObject.activeSelf && GameController.Instance.m_BluePortal.gameObject.activeSelf)
        {
            m_Crosshair.sprite = m_HUDImages[0];
        }
        else if (!GameController.Instance.m_OrangePortal.gameObject.activeSelf)
        {
            m_Crosshair.sprite = m_HUDImages[1];
        }
        else if (!GameController.Instance.m_BluePortal.gameObject.activeSelf)
        {
            m_Crosshair.sprite = m_HUDImages[2];
        }
        else
        {
            m_Crosshair.sprite = m_HUDImages[3];
        }
    }
}
