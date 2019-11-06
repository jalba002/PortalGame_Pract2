using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : MonoBehaviour, IInteractable
{
    public UnityEvent TurnedOn;
    public UnityEvent TurnedOff;

    [System.Serializable]
    public class AudioClips
    {
        public AudioClip m_OnSound;
        public AudioClip m_OffSound;
    }
    public AudioClips m_AudioClips;
    private AudioSource m_AudioSource;

    private bool activated;
    public bool m_Activated
    {
        get { return activated; }
        protected set
        {
            activated = value;
        }
    }
    protected bool m_AlreadyInteracting;

    public void Start()
    {
        m_Activated = false;
        m_AlreadyInteracting = false;
        try
        {
            m_AudioSource = GetComponent<AudioSource>();
            TurnedOn.AddListener(PlayOnSound);
            TurnedOff.AddListener(PlayOffSound);
        }
        catch
        {
            //Debug.Log("This don't have no Audiosource");
        }
    }

    private void PlayOnSound()
    {
        AudioManager.PlayClip(m_AudioSource, m_AudioClips.m_OnSound);
    }

    private void PlayOffSound()
    {
        AudioManager.PlayClip(m_AudioSource, m_AudioClips.m_OffSound);
    }

   

    public virtual bool Interact()
    {
        throw new System.NotImplementedException();
    }

    public bool IsInteractable()
    {
        if (m_AlreadyInteracting)
        {
            return false;
        }
        return true;
    }

}
