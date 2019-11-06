using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool m_IsPlayerDead = false;

    [Header("Recoil Settings")]
    public bool m_EnableRecoil;
    private bool m_AddedRecoil = false;
    private bool m_Shooting = false;
    [HideInInspector] public float m_AccumulatedRecoil;
    [HideInInspector] public bool m_RemovingRecoil;
    [Range(0f, 1f)] public float m_RecoilRecoverySpeed;


    [Header("Camera Settings")]
    public float m_YawRotationalSpeed = 360.0f;
    public float m_PitchRotationalSpeed = 180.0f;
    public float m_MinPitch = -80.0f;
    public float m_MaxPitch = 50.0f;
    public Transform m_PitchControllerTransform;

    [Header("Interactable Settings")]
    public LayerMask m_InteractableLayers;
    public float m_InteractionRange;
    public GameObject m_InteractionScreenPanel;

    [Header("Misc Camera Settings")]
    public Camera m_RenderCamera;
    public bool m_InvertedYaw = false;
    public bool m_InvertedPitch = false;
    public bool m_AngleLocked = false;
    public bool m_AimLocked = false;
    float m_Yaw;
    float m_Pitch;

    [Header("Movement")]
    CharacterController m_CharacterController;
    public float m_Speed = 10.0f;

    [System.Serializable]
    public class KeyBindings
    {
        [Header("Player keybinds")]
        public KeyCode m_UpKeyCode = KeyCode.W;
        public KeyCode m_LeftKeyCode = KeyCode.A;
        public KeyCode m_DownKeyCode = KeyCode.S;
        public KeyCode m_RightKeyCode = KeyCode.D;
        public KeyCode m_RunKeyCode = KeyCode.LeftShift;
        public KeyCode m_JumpKeyCode = KeyCode.Space;
        public KeyCode m_ReloadKeyCode = KeyCode.R;
        public KeyCode m_InteractButton = KeyCode.E;
        public KeyCode m_AimButton = KeyCode.Z;
        [Header("Mouse Bindings")]
        public int m_MouseShootButton = 0;
        public int m_MouseAimButton = 1;
        public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
        public KeyCode m_DebugLockKeyCode = KeyCode.O;

    }
    public KeyBindings KeysDefinition;
    [Header("Weapon Settings")]
    public WeaponScript m_EquippedWeapon;

    [Header("Jumping Settings")]
    public float m_JumpSpeed;
    private bool m_OnGround = false;
    private float m_VerticalSpeed = 0.0f;

    [Header("Running Settings")]
    [Range(1.0f, 2.0f)] public float m_FastSpeedMultiplier;
    [Range(1.0f, 2.0f)] public float m_OnAirSpeedMultiplier;

    // Components Area
    private AudioSource m_AudioSource;

    [Header("Audio Clips")]
    [SerializeField]
    private AudioClip m_WalkingSound;
    [SerializeField]
    private AudioClip m_RunningSound;

    public Coroutine m_RecoilCoroutine;
    public Coroutine m_WaitToRemoveRecoilCoroutine;

    //[Header("UI Settings")]
    //public PlayerHUDUpdater m_PlayerHUDUpdater;

    void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Pitch = m_PitchControllerTransform.localRotation.eulerAngles.x;
        //GameManager.m_PlayerComponents.PlayerHealthManager.m_CharacterDeath.AddListener(SetPlayerDead);
        if (m_AimLocked)
            Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!m_IsPlayerDead)
        {
            PlayerMovement();
            RegisterWeaponInputs();
            //AnalyzeInteractions();
#if UNITY_EDITOR
            UnityEditorChecks();
#endif
        }
    }

    private void LateUpdate()
    {
        if (m_IsPlayerDead) return;
        CameraMovement();
        if (m_EnableRecoil && m_EquippedWeapon != null)
        {
            CorrectRecoil();
        }
    }

    void CameraMovement()
    {
        if (m_AngleLocked) return;

        float l_MouseAxisX = Input.GetAxis("Mouse X");
        m_Yaw -= l_MouseAxisX * m_YawRotationalSpeed * Time.deltaTime * (m_InvertedYaw ? 1 : -1);

        float l_MouseAxisY = Input.GetAxis("Mouse Y");

        m_Pitch -= (l_MouseAxisY * m_PitchRotationalSpeed * Time.deltaTime * (m_InvertedPitch ? -1 : 1));
        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);

        transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
        m_PitchControllerTransform.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);

    }

    void PlayerMovement()
    {
        Vector3 l_Movement = Vector3.zero;
        //Movement Calculations and stuff that could serve if you are on your own engine. 
        float l_YawInRadians = m_Yaw * Mathf.Deg2Rad;
        float l_Yaw90InRadians = (m_Yaw + 90.0f) * Mathf.Deg2Rad;
        Vector3 l_Forward = new Vector3(Mathf.Sin(l_YawInRadians), 0.0f, Mathf.Cos(l_YawInRadians));
        Vector3 l_Right = new Vector3(Mathf.Sin(l_Yaw90InRadians), 0.0f, Mathf.Cos(l_Yaw90InRadians));

        if (Input.GetKey(KeysDefinition.m_UpKeyCode))
            l_Movement = l_Forward;
        else if (Input.GetKey(KeysDefinition.m_DownKeyCode))
            l_Movement = -l_Forward;

        if (Input.GetKey(KeysDefinition.m_RightKeyCode))
            l_Movement += l_Right;
        else if (Input.GetKey(KeysDefinition.m_LeftKeyCode))
            l_Movement -= l_Right;

        if (Input.GetKeyDown(KeysDefinition.m_JumpKeyCode) & m_OnGround)
        {
            m_VerticalSpeed = m_JumpSpeed;
        }
        float l_SpeedMultiplier = 1.0f;
        if (Input.GetKey(KeysDefinition.m_RunKeyCode))
        {
            l_SpeedMultiplier = m_FastSpeedMultiplier;
        }
        else if (Input.GetKey(KeysDefinition.m_RunKeyCode) & m_OnGround)
        {
            l_SpeedMultiplier = m_OnAirSpeedMultiplier;
        }

        l_Movement.Normalize();
        l_Movement = l_Movement * Time.deltaTime * m_Speed * l_SpeedMultiplier;

        m_VerticalSpeed += Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);

        if ((l_CollisionFlags & CollisionFlags.Below) != 0)
        {
            m_OnGround = true;
            m_VerticalSpeed = 0.0f;
        }
        else
            m_OnGround = false;

        if ((l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f)
            m_VerticalSpeed = 0.0f;
    }

#if UNITY_EDITOR
    private void UnityEditorChecks()
    {
        if (Input.GetKeyDown(KeysDefinition.m_DebugLockAngleKeyCode))
            m_AngleLocked = !m_AngleLocked;
        if (Input.GetKeyDown(KeysDefinition.m_DebugLockKeyCode))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
            m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
        }
    }
#endif

    private void RegisterWeaponInputs()
    {
        if (m_EquippedWeapon == null) return;
        if (Input.GetMouseButton(KeysDefinition.m_MouseShootButton))
        {
            if (m_EquippedWeapon.Shoot(GameController.Instance.m_BluePortal))
            {
                //Add recoil here.
                if (m_EnableRecoil)
                {
                    StartCoroutine(AddRecoil());
                    m_Shooting = true;
                }

            }
            else if (m_EquippedWeapon.m_CurrentAmmo > 0 && m_EnableRecoil)
            {
                m_WaitToRemoveRecoilCoroutine = ExecuteCoroutine(m_WaitToRemoveRecoilCoroutine, WaitToRemoveRecoil());
            }
        }
        else if (Input.GetMouseButtonUp(KeysDefinition.m_MouseShootButton) || m_Shooting)
        {
            m_Shooting = false;
        }

        if (Input.GetMouseButton(KeysDefinition.m_MouseAimButton))
        {
            if (m_EquippedWeapon.Shoot(GameController.Instance.m_OrangePortal))
            {
                //Add recoil here.
                if (m_EnableRecoil)
                {
                    StartCoroutine(AddRecoil());
                    m_Shooting = true;
                }

            }
            else if (m_EquippedWeapon.m_CurrentAmmo > 0 && m_EnableRecoil)
            {
                m_WaitToRemoveRecoilCoroutine = ExecuteCoroutine(m_WaitToRemoveRecoilCoroutine, WaitToRemoveRecoil());
            }
        }
        else if (Input.GetMouseButtonUp(KeysDefinition.m_MouseShootButton) || m_Shooting)
        {
            m_Shooting = false;
        }

        if (Input.GetKeyDown(KeysDefinition.m_AimButton))
            m_EquippedWeapon.Aim();
        else if (Input.GetKeyUp(KeysDefinition.m_AimButton))
            m_EquippedWeapon.StopAiming();

        if (Input.GetKeyDown(KeysDefinition.m_ReloadKeyCode))
            m_EquippedWeapon.Reload();
    }

    private void AnalyzeInteractions()
    {
        Ray l_CameraRay = m_RenderCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit l_RaycastHit;
        IInteractable l_InteractableObject;
        if (Physics.Raycast(l_CameraRay, out l_RaycastHit, m_InteractionRange, m_InteractableLayers.value))
        {
            Debug.DrawRay(m_RenderCamera.transform.position, (l_RaycastHit.transform.position - m_RenderCamera.transform.position), Color.red);
            try
            {
                l_InteractableObject = l_RaycastHit.collider.gameObject.GetComponent<IInteractable>();
                if (l_InteractableObject == null)
                {
                    m_InteractionScreenPanel.SetActive(false);
                    return;
                }
            }
            catch
            {
                Debug.Log("Can't interact with object");
                return;
            }
            if (!l_InteractableObject.IsInteractable())
            {
                m_InteractionScreenPanel.SetActive(false);
                return;
            }
            m_InteractionScreenPanel.SetActive(true);
            if (Input.GetKeyDown(KeysDefinition.m_InteractButton))
            {
                m_InteractionScreenPanel.SetActive(l_InteractableObject.Interact());
                m_InteractionScreenPanel.SetActive(false);
            }
        }
        else
        {
            m_InteractionScreenPanel.SetActive(false);
        }
    }

    public void CorrectRecoil()
    {
        if (m_Shooting && m_AddedRecoil)
        {
            m_RemovingRecoil = false;
            if (m_RecoilCoroutine != null)
                StopCoroutine(m_RecoilCoroutine);
        }
        if (!m_Shooting && m_AddedRecoil && !m_RemovingRecoil)
        {
            //Baixa el recoil.
            m_RecoilCoroutine = ExecuteCoroutine(m_RecoilCoroutine, RemoveRecoil());
        }
    }

    private Coroutine ExecuteCoroutine(Coroutine l_CoroutineHolder, IEnumerator l_MethodName)
    {
        if (l_CoroutineHolder != null)
            StopCoroutine(l_CoroutineHolder);
        return StartCoroutine(l_MethodName);
    }

    public IEnumerator RemoveRecoil()
    {
        m_RemovingRecoil = true;
        //float l_OriginalAmount = m_AccumulatedRecoil;
        float l_Amount = m_AccumulatedRecoil * m_RecoilRecoverySpeed;
        while (m_AccumulatedRecoil > 0)
        {
            m_Pitch += l_Amount;
            m_AccumulatedRecoil -= l_Amount;
            yield return null;
        }
        m_AccumulatedRecoil = 0;
        m_AddedRecoil = false;
        m_RemovingRecoil = false;
    }

    public IEnumerator WaitToRemoveRecoil()
    {
        yield return new WaitForSeconds(0.15f);
        m_Shooting = false;
    }

    public IEnumerator AddRecoil()
    {
        float value = m_EquippedWeapon.m_RecoilKick;
        m_AccumulatedRecoil += value;
        float lMouseY = Input.GetAxis("Mouse Y") * m_PitchRotationalSpeed * 0.13f;
        m_AccumulatedRecoil += (lMouseY < 0 ? lMouseY : 0.0f);
        yield return null;
        if (!m_AddedRecoil) m_AddedRecoil = true;
        m_Pitch -= value;
    }

    private void PlayFootstepSounds()
    {
        if (m_OnGround)
        {
            m_AudioSource.clip = Input.GetKey(KeysDefinition.m_RunKeyCode) ? m_RunningSound : m_WalkingSound;
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.Play();
            }
        }
        else
        {
            if (m_AudioSource.isPlaying)
            {
                m_AudioSource.Pause();
            }
        }
    }

    public void SetPlayerDead()
    {

    }

    public void ForceYaw(float l_Value)
    {
        m_Yaw = l_Value;
    }

}