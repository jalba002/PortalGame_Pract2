using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Portal m_MirrorPortal;
    public Transform m_PlayerCamera;
    public Camera m_PortalCamera;

    public Plane m_PortalPlane;

    public float m_NearClipOffset = 0.1f;
    public bool m_PlayerInsideCollider;
    private bool m_TeleportedThisFrame;

    public List<GameObject> m_ObjectsInsideTriggers = new List<GameObject>();

    //Private variables.

    void Start()
    {
        Init();
    }

    void Update()
    {
        UpdatePortalTexture2();
        CheckEveryObjectTeleport(); //CheckTeleport(); //Before it was just CheckTeleport();
    }

    public void Init()
    {
        m_TeleportedThisFrame = false;
        m_PlayerInsideCollider = false;
        m_PortalPlane = new Plane(this.gameObject.transform.forward, this.gameObject.transform.position);
    }

    private void CheckTeleport() //Kinda Legacy
    {
        if (!m_PlayerInsideCollider) return;
        if (!m_PortalPlane.GetSide(GameController.Instance.GetPlayerGameObject().transform.position))
        {
            OriginalTeleport();
        }
    }
    private void CheckEveryObjectTeleport()
    {
        foreach (GameObject objeto in m_ObjectsInsideTriggers)
        {
            if (!m_PortalPlane.GetSide(objeto.transform.position))
            {
                TeleportGameObject(objeto);
            }
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
        PlayerController l_PlayerController = l_Player.GetComponent<PlayerController>();

        Vector3 l_Position = transform.InverseTransformPoint(l_Player.transform.position);
        l_Player.transform.position = m_MirrorPortal.gameObject.transform.TransformPoint(new Vector3(-l_Position.x, l_Position.y, -l_Position.z));

        l_Player.transform.position += m_MirrorPortal.gameObject.transform.forward * 0.1f;

        l_PlayerController.ForceYaw(l_PlayerController.GetYaw() - 180 - (transform.eulerAngles.y - m_MirrorPortal.transform.eulerAngles.y));

        m_TeleportedThisFrame = true;
    }

    public void TeleportGameObject(GameObject l_Object)
    {
        try
        {
            if (!l_Object.GetComponent<Companion>().IsTeleportable()) return;
        }
        catch { }

        Vector3 l_Position = transform.InverseTransformPoint(l_Object.transform.position);
        l_Object.transform.position = m_MirrorPortal.gameObject.transform.TransformPoint(new Vector3(-l_Position.x, l_Position.y, -l_Position.z));
        l_Object.transform.position += m_MirrorPortal.gameObject.transform.forward * 0.1f;

        if (l_Object == GameController.Instance.GetPlayerGameObject())
            l_Object.GetComponent<PlayerController>().ForceYaw(l_Object.GetComponent<PlayerController>().GetYaw() - 180 - (transform.eulerAngles.y - m_MirrorPortal.transform.eulerAngles.y));
        else
        {
            Rigidbody l_Rigidbody = l_Object.GetComponent<Rigidbody>();
            Vector3 l_Velocity = transform.InverseTransformDirection(-l_Rigidbody.velocity);
            l_Rigidbody.velocity = m_MirrorPortal.gameObject.transform.TransformDirection(l_Velocity);
            Vector3 l_Direction = transform.InverseTransformDirection(-transform.forward);
        }

        m_TeleportedThisFrame = true;
    }
    private void UpdatePortalTexture1()
    {
        Vector3 l_ReflectedPosition = transform.InverseTransformPoint(m_PlayerCamera.position);
        Vector3 l_ReflectedDirection = transform.InverseTransformDirection(m_PlayerCamera.forward);
        m_PortalCamera.transform.position = m_MirrorPortal.transform.TransformPoint(new Vector3(-l_ReflectedPosition.x, l_ReflectedPosition.y, -l_ReflectedPosition.z));
        m_PortalCamera.transform.forward = m_MirrorPortal.transform.TransformDirection(new Vector3(-l_ReflectedDirection.x, l_ReflectedDirection.y, -l_ReflectedDirection.z));

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

    public void SetNewPosition(RaycastHit l_SpawnPoint)
    {
        transform.position = l_SpawnPoint.point + (l_SpawnPoint.normal * 0.01f);
        transform.forward = l_SpawnPoint.normal;
        Init();
    }

    public void ObjectInsideCollider(GameObject l_Object, bool isInside)
    {
        if (isInside)
            m_ObjectsInsideTriggers.Add(l_Object);
        else
            m_ObjectsInsideTriggers.Remove(l_Object);
        GameController.Instance.ChangeLayer(l_Object, isInside);
    }
}
