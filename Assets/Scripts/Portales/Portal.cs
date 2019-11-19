using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour, IRestartable
{
    [System.Serializable]
    public class MimicObject
    {
        public MimicObject(GameObject l_Original, GameObject l_Dummy)
        {
            m_Original = l_Original;
            m_Dummy = l_Dummy;
        }
        public GameObject m_Original;
        public GameObject m_Dummy;
    }
    public Portal m_MirrorPortal;
    public Transform m_PlayerCamera;
    public Camera m_PortalCamera;

    public Plane m_PortalPlane;

    public float m_NearClipOffset = 0.1f;
    public bool m_PlayerInsideCollider;
    private bool m_TeleportedThisFrame;

    public LaserPortal m_Laser;

    public List<GameObject> m_ObjectsInsideTriggers = new List<GameObject>();
    private List<MimicObject> m_MimicableObjects = new List<MimicObject>();


    public Vector3 m_MaxScale;
    public Vector3 m_MinScale;

    public Vector3 m_OriginalScale;
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
        }
    }

    //Private variables.
    private void Awake()
    {
        m_OriginalScale = this.transform.localScale;
        m_MinScale = m_OriginalScale * 0.5f;
        m_MaxScale = m_OriginalScale * 2f;
        m_CurrentScale = m_OriginalScale;
    }
    void Start()
    {
        Init();
        this.gameObject.SetActive(false);
    }

    void Update()
    {
        UpdatePortalTexture2();
        CheckEveryObjectTeleport(); 
    }

    public void Init()
    {
        m_TeleportedThisFrame = false;
        m_PlayerInsideCollider = false;
        m_PortalPlane = new Plane(this.gameObject.transform.forward, this.gameObject.transform.position);
    }

    public void SetNewScale(Vector3 l_NewScale)
    {
        m_CurrentScale = l_NewScale;
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
        foreach (MimicObject l_Dummy in m_MimicableObjects)
        {
            MimicPosition(l_Dummy.m_Original, l_Dummy.m_Dummy);
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

        if (l_Object == GameController.Instance.GetPlayerGameObject())
            l_Object.GetComponent<PlayerController>().ForceYaw(l_Object.GetComponent<PlayerController>().GetYaw() - 180 - (transform.eulerAngles.y - m_MirrorPortal.transform.eulerAngles.y));
        else
        {
            Rigidbody l_Rigidbody = l_Object.GetComponent<Rigidbody>();
            Vector3 l_Velocity = transform.InverseTransformDirection(-l_Rigidbody.velocity);
            l_Velocity = new Vector3(l_Velocity.x, -l_Velocity.y, l_Velocity.z);
            l_Rigidbody.velocity = m_MirrorPortal.gameObject.transform.TransformDirection(l_Velocity);
            Vector3 l_Direction = transform.InverseTransformDirection(-transform.forward);
            l_Rigidbody.gameObject.transform.forward = l_Direction;
            //Testing of size changes

            l_Object.GetComponent<Companion>().SetNewScale(l_Object.transform.localScale *= (this.m_MirrorPortal.transform.localScale.x / this.gameObject.transform.localScale.x));
        }

        m_TeleportedThisFrame = true;
    }
    private void UpdatePortalTexture1()
    {
        Vector3 l_ReflectedPosition = transform.InverseTransformPoint(m_PlayerCamera.position);
        Vector3 l_ReflectedDirection = transform.InverseTransformDirection(m_PlayerCamera.forward);
        m_PortalCamera.transform.position = m_MirrorPortal.transform.TransformPoint(new Vector3(-l_ReflectedPosition.x, l_ReflectedPosition.y, -l_ReflectedPosition.z));
        m_PortalCamera.transform.forward = m_MirrorPortal.transform.TransformDirection(new Vector3(-l_ReflectedDirection.x, l_ReflectedDirection.y, -l_ReflectedDirection.z));

        m_PortalCamera.nearClipPlane = m_NearClipOffset;//Vector3.Distance(m_PortalCamera.transform.position, this.transform.position) + m_NearClipOffset;
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

        m_PortalCamera.nearClipPlane = Vector3.Distance(m_PortalCamera.transform.position, this.transform.position); //+ m_NearClipOffset;
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
        {
            AddAndCreateDummy(l_Object, true);
            GameController.Instance.ChangeLayer(l_Object, true);
        }
        else
            StartCoroutine(ResetCollisions(l_Object));

    }

    public IEnumerator ResetCollisions(GameObject l_Object)
    {
        AddAndCreateDummy(l_Object, false);
        yield return null;
        if (m_MirrorPortal.m_PortalPlane.GetDistanceToPoint(l_Object.transform.position) > 1f)
        {
            GameController.Instance.ChangeLayer(l_Object, false);
        }
    }

    public void Restart()
    {
        this.gameObject.SetActive(false);
    }

    private void MimicPosition(GameObject l_Original, GameObject l_Dummy)
    {
        Vector3 l_Position = transform.InverseTransformPoint(l_Original.transform.position);
        l_Dummy.transform.position = m_MirrorPortal.gameObject.transform.TransformPoint(new Vector3(-l_Position.x, l_Position.y, -l_Position.z));

        l_Dummy.transform.rotation = l_Original.transform.rotation;
        if (l_Original != GameController.Instance.GetPlayerGameObject())
        {
            Rigidbody l_Rigidbody = l_Dummy.GetComponent<Rigidbody>();
            Rigidbody l_OGRigidbody = l_Original.GetComponent<Rigidbody>();
            Vector3 l_Velocity = transform.InverseTransformDirection(-l_OGRigidbody.velocity);
            l_Velocity = new Vector3(l_Velocity.x, -l_Velocity.y, l_Velocity.z);
            l_Rigidbody.velocity = m_MirrorPortal.gameObject.transform.TransformDirection(l_Velocity);
            Vector3 l_Direction = transform.InverseTransformDirection(transform.forward);
            l_Rigidbody.gameObject.transform.forward = l_Direction;
        }
    }

    private void AddAndCreateDummy(GameObject l_Object, bool l_Add)
    {
        if (l_Add)
        {
            m_ObjectsInsideTriggers.Add(l_Object);
            if (l_Object != GameController.Instance.GetPlayerGameObject()) return;
            foreach (MimicObject m_MimicObject in m_MimicableObjects)
            {
                if (m_MimicObject.m_Original == l_Object || l_Object == m_MimicObject.m_Dummy)
                    return;
            }
            GameObject l_Dummy = Instantiate(l_Object, GameController.Instance.m_DestroyInstantiatedObjectsParent);
            if (l_Object == GameController.Instance.GetPlayerGameObject())
            {
                l_Dummy.GetComponentInChildren<PlayerController>().enabled = false;
                l_Dummy.GetComponentInChildren<WeaponScript>().gameObject.SetActive(false);
                Camera[] l_DummyCams = l_Dummy.GetComponentsInChildren<Camera>();
                foreach (Camera cam in l_DummyCams)
                {
                    cam.gameObject.SetActive(false);
                }
            }
            else
            {
                l_Dummy.GetComponent<Companion>().SetTeleport(false);
            }
            m_MimicableObjects.Add(new MimicObject(l_Object, l_Dummy));
        }
        else
        {
            m_ObjectsInsideTriggers.Remove(l_Object);
            if (l_Object != GameController.Instance.GetPlayerGameObject()) return;
            foreach (MimicObject m_MimicObject in m_MimicableObjects)
            {
                if (m_MimicObject.m_Original == l_Object || l_Object == m_MimicObject.m_Dummy)
                {
                    m_MimicObject.m_Dummy.SetActive(false);
                    m_MimicableObjects.Remove(m_MimicObject);
                    return;
                }

            }
        }
    }

}
