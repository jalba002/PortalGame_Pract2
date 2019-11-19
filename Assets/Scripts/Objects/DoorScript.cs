using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    public enum DoorMode
    {
        Proximity,
        Score,
        Button,
        MultipleButtons,
        External
    }
    public DoorMode m_DoorMode;

    [System.Serializable]
    public class SoundClips
    {
        public AudioClip m_OpenClip;
        public AudioClip m_CloseClip;
        public AudioClip m_UnlockClip;
    }
    public SoundClips m_SoundClips;

    public List<ButtonInteractable> m_ConnectedButtons = new List<ButtonInteractable>();

    private bool m_ScoreUnlocked;
    private bool m_Opened;
    private Animation m_Animation;
    private AudioSource m_AudioSource;

    private void Start()
    {
        m_Opened = false;
        m_ScoreUnlocked = false;
        m_Animation = GetComponent<Animation>();
        m_AudioSource = GetComponent<AudioSource>();
    }

    private void ScoreUnlock()
    {
        if (m_DoorMode != DoorMode.Score) return;
        m_ScoreUnlocked = true;
        AudioManager.PlayClip(m_AudioSource, m_SoundClips.m_UnlockClip);

    }

    public void Open()
    {
        bool l_AbleToOpen = AbleToOpen();

        if (!m_Opened && l_AbleToOpen)
        {
            m_Animation.Play("PortalOpen", PlayMode.StopAll);
            AudioManager.PlayClip(m_AudioSource, m_SoundClips.m_OpenClip);
            m_Opened = true;
        }
    }

    public void Close()
    {
        bool l_AbleToClose = AbleToClose();

        if (m_Opened && l_AbleToClose)
        {
            m_Animation.Play("PortalClose", PlayMode.StopAll);
            AudioManager.PlayClip(m_AudioSource, m_SoundClips.m_CloseClip);
            m_Opened = false;
        }
    }

    private bool AbleToOpen()
    {
        bool l_AbleToOpen = false;
        switch (m_DoorMode)
        {
            case DoorMode.Proximity:
                l_AbleToOpen = true;
                break;
            case DoorMode.Score:
                l_AbleToOpen = m_ScoreUnlocked;
                break;
            case DoorMode.Button:
                l_AbleToOpen = CheckAllRegisteredButtons();
                break;
            default:
                Debug.LogWarning("Won't open in this condition!");
                break;
        }
        return l_AbleToOpen;
    }

    private bool AbleToClose()
    {
        bool l_AbleToClose = false;
        switch (m_DoorMode)
        {
            case DoorMode.Proximity:
                l_AbleToClose = true;
                break;
            case DoorMode.Score:
                l_AbleToClose = m_ScoreUnlocked;
                break;
            case DoorMode.Button:
                l_AbleToClose = !CheckAllRegisteredButtons();
                break;
            default:
                Debug.LogWarning("Won't close in this condition!");
                break;
        }
        return l_AbleToClose;
    }

    /*public bool KeyObtained(DoorKey theKey)
    {
        if (m_DoorMode != DoorMode.Key) return false;
        if (theKey == m_AttachedKey)
        {
            m_KeyUnlocked = true;
            m_AudioSource.Play();
        }
        return m_KeyUnlocked;
    }*/

    private bool CheckAllRegisteredButtons()
    {
        foreach (ButtonInteractable l_ConnectedButton in m_ConnectedButtons)
        {
            if (!l_ConnectedButton.IsActivated())
            {
                return false;
            }
        }
        return true;
    }

    public void ForceOpen()
    {
        if (!m_Opened)
        {
            m_Animation.Play("PortalOpen", PlayMode.StopAll);
            AudioManager.PlayClip(m_AudioSource, m_SoundClips.m_OpenClip);
            m_Opened = true;
        }
    }
}
