using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal m_MirrorPortal;
    public Transform m_PlayerCamera;
    public Camera m_PortalCamera;

    public float m_NearClipOffset = 0.5f;

    void Start()
    {

    }

    void Update()
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
}
