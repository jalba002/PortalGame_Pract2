using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponScript : MonoBehaviour
{
    public enum ShootingType
    {
        Hitscan, Projectile, Portal
    }

    public PlayerController m_Owner;
    Animator m_Animator;

    [Header("Weapon Camera")]
    public Camera m_GunCamera;
    public Camera m_MainCamera;

    [Header("Camera Options")]
    public float m_FOVSpeed;
    public float m_AimGunFOV;
    public float m_AimMainFOV;
    private float m_DefaultGunFOV;
    private float m_DefaultMainFOV;

    [Header("Recoil Settings")]
    public float m_RecoilKick;

    [Header("Weapon Sway")]
    public float m_SwayAmount;
    public float m_MaxSwayAmount;
    public float m_SwaySmoothValue;
    private Vector3 m_InitialSwayPosition;

    [Header("Ammo Parameters")]
    public int m_MaxAmmo;
    public int m_MaxCarriedAmmo;
    private int currentAmmo;
    public int m_CurrentAmmo
    {
        get { return currentAmmo; }
        private set
        {
            currentAmmo = value;
            //UpdateHUD();
        }
    }
    private int currentCarriedAmmo;
    public int m_CurrentCarriedAmmo
    {
        get { return currentCarriedAmmo; }
        private set
        {
            currentCarriedAmmo = value;
            //UpdateHUD();
        }
    }

    [Header("Weapon Settings")]
    public bool m_EnableRecoil;
    public bool m_EnableDecals;
    public bool m_AutoReload;
    public int m_DamagePerShot;
    public LayerMask m_ShootLayerMask;
    public LayerMask m_PortalLayerMask;
    [Range(1, 10)] public int m_ShootChecksPerFrame;
    public ShootingType m_ShootingType = ShootingType.Hitscan;
    [Tooltip("Rate of fire measured in RPM aka. Rounds Per Minute")] public float m_FireRate;
    public float m_MaxRange;
    private float m_FireTimer;
    private float m_RealROF;
    private bool m_AbleToShoot;
    private bool m_Reloading = false;
    private bool m_IsAiming = false;


    [Header("Muzzleflash Light Settings")]
    public Light m_MuzzleFlash;
    public float m_LightDuration = 0.02f;

    [Header("Cheat Settings")]
    private bool m_InfiniteAmmo = true;
    //End of weapon settings.

    [Header("Audio Sources")]
    public AudioSource m_ShootAudioSource;
    public AudioSource m_MainAudioSource;


    [System.Serializable]
    public class Prefabs
    {
        [Header("Prefabs")]
        public GameObject m_BulletPrefab;
        public GameObject m_CasingPrefab;
        public GameObject m_WeaponDecals;

    }
    public Prefabs PrefabsList;

    [System.Serializable]
    public class Spawnpoints
    {
        [Header("Spawnpoints")]
        public Transform m_CasingSpawnPoint;
        public Transform m_BulletSpawnPoint;
    }
    public Spawnpoints SpawnpointsList;

    /*[System.Serializable]
    public class ParticleSystems
    {
        public ParticleSystem m_MuzzleParticles;
        //public ParticleSystem m_SparkParticles;
    }
    public ParticleSystems m_ParticleSystems;*/

    [System.Serializable]
    public class SoundClips
    {
        public AudioClip m_AimSound;
        public AudioClip m_ShootSound;
        public AudioClip m_ReloadSoundOutOfAmmo;
        public AudioClip m_ReloadSoundAmmoLeft;
    }
    public SoundClips SoundClipList;
    private bool soundHasPlayed = false;

    private Coroutine m_StartZoom;
    private Coroutine m_StopZoom;
    private Coroutine m_StopRecoil;

    public GameObject m_CrosshairGO;

    void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }

    void Start()
    {
        m_CurrentAmmo = m_MaxAmmo;
        m_CurrentCarriedAmmo = 1;
        m_InitialSwayPosition = transform.localPosition;
        m_ShootAudioSource.clip = SoundClipList.m_ShootSound;
        m_RealROF = 60.0f / m_FireRate;

        m_FireTimer = 0.0f;
        m_DefaultGunFOV = m_GunCamera.fieldOfView;
        m_DefaultMainFOV = m_MainCamera.fieldOfView;

        //UpdateHUD();
    }

    void Update()
    {
        WeaponSway();
        UpdateAbleToShoot();
        //CheckWeaponWalking();
    }

    private void WeaponSway()
    {
        float l_MovementX = -Input.GetAxis("Mouse X") * m_SwayAmount;
        l_MovementX = Mathf.Clamp(l_MovementX, -m_MaxSwayAmount, m_MaxSwayAmount);

        float l_MovementY = -Input.GetAxis("Mouse Y") * m_SwayAmount;
        l_MovementY = Mathf.Clamp(l_MovementY, -m_MaxSwayAmount, m_MaxSwayAmount);

        Vector3 l_FinalSwayPosition = new Vector3((!m_IsAiming ? l_MovementX : 0), (!m_IsAiming ? l_MovementY : 0), 0);
        transform.localPosition = Vector3.Lerp(transform.localPosition, l_FinalSwayPosition + m_InitialSwayPosition, Time.deltaTime * m_SwaySmoothValue);
    }

    public bool Shoot(Portal l_Portal)
    {
        bool l_ShootThisFrame = TrueShoot(l_Portal);
        if (l_ShootThisFrame)
        {
            //Do stuff
            m_AbleToShoot = false;
            ReduceAmmo(1);
            m_FireTimer = 0.0f;
            AudioManager.PlayClip(m_ShootAudioSource, SoundClipList.m_ShootSound);
            return true;
        }
        return l_ShootThisFrame;
    }

    private bool TrueShoot(Portal l_Portal)
    {
        if (CanShoot())
        {
            if (!m_IsAiming)
            {
                //m_Animator.Play("Fire", 0, 0f);
                //m_ParticleSystems.m_MuzzleParticles.Emit(1);
                //StartCoroutine(MuzzleFlash());
                //Disable Crosshair too. TO-DO
            }
            else
            {
                //m_Animator.Play("Aim Fire", 0, 0f);
                //m_ParticleSystems.m_MuzzleParticles.Emit(1);
            }
            switch (m_ShootingType)
            {
                case ShootingType.Hitscan:
                    //return false;
                    //Instantiate the whole thingy.
                    Ray l_CameraRay = m_GunCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
                    RaycastHit l_RaycastHit;
                    if (Physics.Raycast(l_CameraRay, out l_RaycastHit, m_MaxRange, m_ShootLayerMask.value))
                    {
                        /*EffectsManager.CreateDecals(PrefabsList.m_WeaponDecals, l_RaycastHit.point, l_RaycastHit.normal, l_RaycastHit.transform);
                        IDamageable l_HitHPInterface;
                        try
                        {
                            l_HitHPInterface = l_RaycastHit.collider.transform.GetComponent<IDamageable>();
                            l_HitHPInterface.DealDamage(m_DamagePerShot, l_RaycastHit.collider);
                        }
                        catch
                        {
                            //Debug.Log("Hit is not an enemy!");
                        }*/
                    }
                    break;
                case ShootingType.Projectile:
                    Debug.LogWarning("No projectile method programmed yet!");
                    break;
                case ShootingType.Portal:
                    Ray l_CameraPortalRay = m_GunCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
                    RaycastHit l_RayHit;
                    if (Physics.Raycast(l_CameraPortalRay, out l_RayHit, m_MaxRange, m_ShootLayerMask.value))
                    {
                        if (l_RayHit.collider.gameObject.tag == "PrintableWall")
                        {
                            GameController.Instance.m_PortalCheckerPoint.transform.position = l_RayHit.point + (l_RayHit.normal * 0.05f);
                            GameController.Instance.m_PortalCheckerPoint.transform.forward = -l_RayHit.normal;

                            PortalSpawner.SpawnPortal(l_Portal, l_RayHit, GameController.Instance.m_PortalCheckersList);
                        }
                    }
                    break;
                default:
                    Debug.Log("Did you forget to set the Shooting type?");
                    break;
            }
            return true;
        }
        else if (m_CurrentAmmo == 0)
        {
            //Play No ammo sound.
            if (m_AutoReload)
                Reload();
            return false;
        }
        return false;
    }

    public void CreatePreview()
    {
        Ray l_CameraPortalRay = m_GunCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit l_RayHit;
        if (Physics.Raycast(l_CameraPortalRay, out l_RayHit, m_MaxRange, m_ShootLayerMask.value))
        {
            if (l_RayHit.collider.gameObject.tag == "PrintableWall")
            {
                GameController.Instance.m_PortalCheckerPoint.transform.position = l_RayHit.point + (l_RayHit.normal * 0.05f);
                GameController.Instance.m_PortalCheckerPoint.transform.forward = -l_RayHit.normal;

                PortalSpawner.CreatePortalPreview(GameController.Instance.m_PortalPreview, true, l_RayHit, GameController.Instance.m_PortalCheckersList);
            }
        }
    }

    public void HidePreview()
    {
        GameController.Instance.m_PortalPreview.gameObject.SetActive(false);
    }

    public void Aim()
    {
        if (m_Reloading) return;
        //m_Animator.SetBool("Aim", true);
        m_IsAiming = true;
        m_StartZoom = ExecuteCoroutine(m_StartZoom, StartZoom());
        //m_CrosshairGO.SetActive(false);
        if (!soundHasPlayed)
        {
            AudioManager.PlayClip(m_MainAudioSource, SoundClipList.m_AimSound);
            soundHasPlayed = true;
        }
    }

    public void StopAiming()
    {
        if (m_Reloading || !m_IsAiming) return;
        m_IsAiming = false;

        //m_Animator.SetBool("Aim", false);
        m_StopZoom = ExecuteCoroutine(m_StopZoom, StopZoom());
        soundHasPlayed = false;
        //m_CrosshairGO.SetActive(true);
    }

    public void Reload()
    {
        if (m_Reloading || m_CurrentCarriedAmmo <= 0)
        {
            return;
        }
        if (m_CurrentAmmo == 0)
        {
            StopAiming();
            //m_Animator.Play("Reload Out Of Ammo", 0, 0f);
            AudioManager.PlayClip(m_MainAudioSource, SoundClipList.m_ReloadSoundOutOfAmmo);
            m_Reloading = true;
        }
        else if (m_CurrentAmmo < m_MaxAmmo)
        {
            StopAiming();
            //m_Animator.Play("Reload Ammo Left", 0, 0f);
            AudioManager.PlayClip(m_MainAudioSource, SoundClipList.m_ReloadSoundAmmoLeft);
            m_Reloading = true;
        }
    }

    private void PlayAnimation(string l_Animation, int l_Layer, float l_Time)
    {
        /*
        if (m_Animator == null) return;
        m_Animator.Play(l_Animation, l_Layer, l_Time);
        
        */
    }

    private void ReduceAmmo(int l_Value)
    {
        if (m_InfiniteAmmo) return;
        m_CurrentAmmo -= l_Value;
    }

    public void ReloadOutOfAmmo()
    {
        m_Reloading = false;
        int l_AmmoDifference = m_MaxAmmo - m_CurrentAmmo - 1;
        int l_AmmoSum = Mathf.Min(l_AmmoDifference, m_CurrentCarriedAmmo);
        m_CurrentAmmo += l_AmmoSum;
        m_CurrentCarriedAmmo -= l_AmmoSum;
    }

    public void ReloadWithAmmo()
    {
        m_Reloading = false;
        int l_AmmoDifference = m_MaxAmmo - m_CurrentAmmo;
        int l_AmmoSum = Mathf.Min(l_AmmoDifference, m_CurrentCarriedAmmo);
        m_CurrentAmmo += l_AmmoSum;
        m_CurrentCarriedAmmo -= l_AmmoSum;
    }

    public bool RestoreAmmo(int l_Amount)
    {
        if (m_CurrentCarriedAmmo >= m_MaxCarriedAmmo)/* || GameManager.m_PlayerComponents.PlayerController.m_EquippedWeapon.isActiveAndEnabled == false)*/ return false;

        int l_Difference = m_MaxCarriedAmmo - m_CurrentCarriedAmmo;
        int l_RestoredValue = Mathf.Min(l_Difference, l_Amount);
        m_CurrentCarriedAmmo += l_RestoredValue;
        if (m_CurrentCarriedAmmo > m_MaxCarriedAmmo) m_CurrentCarriedAmmo = m_MaxCarriedAmmo;
        //Debug.Log("Restored " + l_RestoredValue + " ammo.");
        return true;
    }

    private void UpdateAbleToShoot()// this checks X times a frame if it must shoot or not.
    {
        if (m_AbleToShoot) return;
        for (int i = 0; i < m_ShootChecksPerFrame; i++)
        {
            m_FireTimer += (Time.deltaTime * (1.0f / m_ShootChecksPerFrame));
            if (m_FireTimer >= m_RealROF)
            {
                m_AbleToShoot = true;
                return;
            }
        }
    }

    public bool CanShoot()
    {
        return m_CurrentAmmo > 0 && m_AbleToShoot && !m_Reloading;
    }

    private IEnumerator StartZoom()
    {
        if (m_StopZoom != null)
            StopCoroutine(m_StopZoom);
        while (m_GunCamera.fieldOfView > m_AimGunFOV && m_MainCamera.fieldOfView > m_AimMainFOV)
        {
            m_GunCamera.fieldOfView = Mathf.Lerp(m_GunCamera.fieldOfView, m_AimGunFOV, m_FOVSpeed * Time.deltaTime);
            m_MainCamera.fieldOfView = Mathf.Lerp(m_MainCamera.fieldOfView, m_AimMainFOV, m_FOVSpeed * Time.deltaTime);
            yield return null;
        }
    }

    private IEnumerator StopZoom()
    {
        if (m_StartZoom != null)
            StopCoroutine(m_StartZoom);
        while (m_GunCamera.fieldOfView < m_DefaultGunFOV && m_MainCamera.fieldOfView < m_DefaultMainFOV)
        {
            m_GunCamera.fieldOfView = Mathf.Lerp(m_GunCamera.fieldOfView, m_DefaultGunFOV, m_FOVSpeed * Time.deltaTime);
            m_MainCamera.fieldOfView = Mathf.Lerp(m_MainCamera.fieldOfView, m_DefaultMainFOV, m_FOVSpeed * Time.deltaTime);
            yield return null;
        }
    }

    /*private IEnumerator StopAllRecoil()
    {
        yield return new WaitForSeconds(0.12f);
        m_Owner.m_CurrentRecoil = 0.0f;
    }*/

    private Coroutine ExecuteCoroutine(Coroutine l_CoroutineHolder, IEnumerator l_MethodName)
    {
        if (l_CoroutineHolder != null)
            StopCoroutine(l_CoroutineHolder);
        return StartCoroutine(l_MethodName);
    }

    /*private void CheckWeaponWalking()
    {
        if (Input.GetKey(KeyCode.W) ||
            Input.GetKey(KeyCode.A) ||
            Input.GetKey(KeyCode.S) ||
            Input.GetKey(KeyCode.D))
        {
            m_Animator.SetBool("Walk", true);
        }
        else
        {
            m_Animator.SetBool("Walk", false);
        }
    }*/

    /*private void UpdateHUD()
    {
        try
        {
            GameController.Instance.m_PlayerComponents.m_HUDUpdater.UpdateHUD();
        }
        catch
        {
            Debug.LogError("Something went wrong when updating the HUD");
        }
    }*/

}