using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal m_MirrorPortal;
    public Transform m_PlayerCamera;
    public Camera m_PortalCamera;

    public Plane m_PortalPlane;
    public LayerMask m_TeleportLayerMask;

    public float m_NearClipOffset = 0.5f;
    public bool m_PlayerInsideCollider;
    //Private variables.

    void Start()
    {
        Init();
    }

    void Update()
    {
        UpdatePortalTexture2();
        CheckTeleport();
    }

    public void Init()
    {
        m_PortalPlane = new Plane(this.gameObject.transform.forward, this.gameObject.transform.position);
        m_PlayerInsideCollider = false;
    }

    private void CheckTeleport()
    {
        if (!m_PlayerInsideCollider) return;
        if (!m_PortalPlane.GetSide(GameController.Instance.GetPlayerGameObject().transform.position))
        {
            OriginalTeleport();//TeleportPlayer();
        }
    }

    private void TeleportPlayer()
    {
        GameObject l_Player = GameController.Instance.GetPlayerGameObject();
        Vector3 l_PlayerLocalPosition = this.transform.InverseTransformPoint(l_Player.transform.position);

        l_Player.transform.position = m_MirrorPortal.transform.position + (m_MirrorPortal.transform.forward * 0.1f);
        l_Player.GetComponent<PlayerController>().ForceYaw(l_Player.transform.eulerAngles.y - (m_MirrorPortal.transform.eulerAngles.y - this.transform.eulerAngles.y));
    }
    public void OriginalTeleport()
    {
        GameObject l_Player = GameController.Instance.GetPlayerGameObject();

        Vector3 l_Position = transform.InverseTransformPoint(l_Player.transform.position);
        l_Player.transform.position = m_MirrorPortal.transform.TransformPoint(new Vector3(-l_Position.x, l_Position.y, -l_Position.z) + (m_MirrorPortal.transform.forward * 0.01f));

        /*Vector3 l_Direction = transform.InverseTransformDirection(-transform.forward);
        l_Player.transform.forward = m_MirrorPortal.transform.TransformDirection(l_Direction);*/

        l_Player.GetComponent<PlayerController>().ForceYaw(l_Player.transform.eulerAngles.y - (m_MirrorPortal.transform.eulerAngles.y - this.transform.eulerAngles.y));
    }

    private void UpdatePortalTexture1()
    {
        Vector3 l_ReflectedPosition = m_MirrorPortal.transform.InverseTransformPoint(m_PlayerCamera.position);
        Vector3 l_ReflectedDirection = m_MirrorPortal.transform.InverseTransformDirection(m_PlayerCamera.forward);
        m_PortalCamera.transform.position = m_MirrorPortal.transform.TransformPoint(l_ReflectedPosition);
        m_PortalCamera.transform.forward = m_MirrorPortal.transform.TransformDirection(l_ReflectedDirection);

        m_PortalCamera.nearClipPlane = Vector3.Distance(m_PortalCamera.transform.position, this.transform.position) + m_NearClipOffset;
    }

    private void UpdatePortalTexture2()
    {
        Vector3 l_EulerAngles = transform.rotation.eulerAngles;
        Quaternion l_Rotation = Quaternion.Euler(l_EulerAngles.x, l_EulerAngles.y + 180.0f, l_EulerAngles.z);
        Matrix4x4 l_WorldMatrix = Matrix4x4.TRS(transform.position, l_Rotation, transform.localScale);

        Vector3 l_ReflectedPosition = l_WorldMatrix.inverse.MultiplyPoint3x4(m_PlayerCamera.position);
        Vector3 l_ReflectedDirection = l_WorldMatrix.inverse.MultiplyVector(m_PlayerCamera.forward);
        m_MirrorPortal.m_PortalCamera.transform.position = m_MirrorPortal.transform.TransformPoint(l_ReflectedPosition);
        m_MirrorPortal.m_PortalCamera.transform.forward = m_MirrorPortal.transform.TransformDirection(l_ReflectedDirection);

        m_PortalCamera.nearClipPlane = Vector3.Distance(m_PortalCamera.transform.position, this.transform.position) + m_NearClipOffset;
    }

    public void PlayerInsideCollider(bool l_Inside)
    {
        m_PlayerInsideCollider = l_Inside;
        GameController.Instance.PlayerIgnoreLayers(m_PlayerInsideCollider);
    }
}
