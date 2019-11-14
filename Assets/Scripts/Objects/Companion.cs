using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public class Companion : MonoBehaviour, IPickable, IRestartable
{
    private Vector3 currentScale;
    public Vector3 m_CurrentScale
    {
        get
        {
            return currentScale;
        }
        set
        {
            Vector3 newValue = value;
            newValue = Vector3.Max(newValue, m_MinScale);
            newValue = Vector3.Min(newValue, m_MaxScale);
            currentScale = newValue;
            this.transform.localScale = currentScale;
            m_OnSizeChange.Invoke();
        }
    }
    public UnityEvent m_OnSizeChange;

    public Vector3 m_MaxScale;
    public Vector3 m_MinScale;
    public bool m_PickedUp;

    private Vector3 m_OriginalScale;
    private bool m_Teleportable;

    private Rigidbody m_Rigidbody;

    [Header("Restart Properties")]
    Vector3 m_OriginalPosition;
    Quaternion m_OriginalRotation;
    bool m_OriginallySetActive;


    protected virtual void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();

        m_OriginalPosition = this.gameObject.transform.position;
        m_OriginalRotation = this.gameObject.transform.rotation;
        m_OriginallySetActive = this.gameObject.activeSelf;

        m_Teleportable = true;
        m_OriginalScale = this.transform.localScale;
        m_MinScale = m_OriginalScale * 0.5f;
        m_MaxScale = m_OriginalScale * 2f;
        m_CurrentScale = m_OriginalScale;
    }

    protected virtual void Start()
    {
        m_OnSizeChange.Invoke();
    }

    public void SetTeleport(bool l_Activate)
    {
        m_Teleportable = l_Activate;
    }

    public void SetNewScale(Vector3 l_NewScale)
    {
        m_CurrentScale = l_NewScale;
    }

    public bool IsTeleportable()
    {
        return m_Teleportable;
    }

    public void Restart()
    {
        this.gameObject.transform.position = m_OriginalPosition;
        this.gameObject.transform.rotation = m_OriginalRotation;
        m_Rigidbody.velocity = Vector3.zero;
        m_Rigidbody.angularVelocity = Vector3.zero;
    }
}
