using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HealthManager : MonoBehaviour, IDamageable, IRestartable
{
    [Header("General Settings")]
    public bool m_DisableOnDeath;
    public bool m_SpawnLootOnDeath;

    [Header("Health Settings")]
    public int m_MaxHealth;
    public Gradient m_Gradient;
    public Color ColorFromGradient(float value) //Between 0 to 1
    {
        return m_Gradient.Evaluate(value);
    }

    [Header("Shield Settings")]
    public int m_MaxShield;
    [Range(0.0f, 1.0f)] public float m_ShieldAbsorbption;
    private float m_HealthAbsorbption;

    [Header("Events")]
    public UnityEvent m_OnDamageTaken;
    public UnityEvent m_CharacterDeath;
    public UnityEvent m_RespawnCharacter;

    private bool m_OwnerIsPlayer;

    private int currentHealth;
    public int m_CurrentHealth
    {
        get { return currentHealth; }
        private set
        {
            currentHealth = value;
        }
    }
    private int currentShield;
    public int m_CurrentShield
    {
        get { return currentShield; }
        private set
        {
            currentShield = value;
        }
    }

    #region Interfaces
    public bool m_Activated { get; set; }
    public Vector3 m_InitialPosition { get; set; }
    public Quaternion m_InitialRotation { get; set; }
    #endregion
    public int m_StoredHealth;
    public int m_StoredShield;

    [Header("UI Images")]
    public Image m_CanvasHealthBar;
    public Image m_CanvasShieldBar;

    public bool m_Invulnerable = false;

    private GameObject m_AttachedGameObject;
    private bool m_IsAttachedCharacterDead;

    void Start()
    {
        m_AttachedGameObject = this.gameObject;

        m_OwnerIsPlayer = m_AttachedGameObject == GameController.Instance.GetPlayerGameObject();

        m_CurrentHealth = m_MaxHealth;
        m_CurrentShield = m_MaxShield;

        m_HealthAbsorbption = 1.0f - m_ShieldAbsorbption;
        m_IsAttachedCharacterDead = (m_CurrentHealth <= 0);

        //Register into the Events
        m_OnDamageTaken.AddListener(CheckDeath);
        if (!m_OwnerIsPlayer)
        {
            m_OnDamageTaken.AddListener(UpdateCanvas);
            UpdateCanvas();
        }
        m_CharacterDeath.AddListener(OnCharacterDeath);
        m_RespawnCharacter.AddListener(OnCharacterRespawn);

        UpdateValues();
    }

    public void DealDamage(int l_Amount, Collider l_ColliderHit)
    {
        if (m_Invulnerable) return;

        int l_Damage = (int)l_Amount;
        if (m_IsAttachedCharacterDead) return;
        if (l_Damage <= 0)
        {
            Debug.LogError("Someone sent incorrect damage values to " + this.gameObject.name);
            return;
        }
        if (m_CurrentShield > 0)
        {
            m_CurrentHealth -= (int)(l_Damage * m_HealthAbsorbption);
            m_CurrentShield -= (int)Mathf.Ceil(l_Damage * m_ShieldAbsorbption);
            if (m_CurrentShield < 0) m_CurrentShield = 0;
        }
        else
        {
            m_CurrentHealth -= l_Damage;
        }
        m_OnDamageTaken.Invoke();
        /*if (!m_OwnerIsPlayer)
            Debug.Log(this.gameObject.name + " stats: " + m_CurrentHealth + ", " + m_CurrentShield);*/
    }

    //Getters
    public int GetCurrentHealth()
    {
        return m_CurrentHealth;
    }
    public int GetCurrentShield()
    {
        return m_CurrentShield;
    }

    public bool RestoreHealth(int l_Amount)
    {
        if (m_CurrentHealth >= m_MaxHealth) return false;

        int l_Difference = m_MaxHealth - m_CurrentHealth;
        m_CurrentHealth += Mathf.Min(l_Difference, l_Amount);
        if (m_CurrentHealth > m_MaxHealth) m_CurrentHealth = m_MaxHealth;

        return true;
    }

    public bool RestoreShield(int l_Amount)
    {
        if (m_CurrentShield >= m_MaxShield) return false;

        int l_Difference = m_MaxShield - m_CurrentShield;
        m_CurrentShield += Mathf.Min(l_Difference, l_Amount);
        if (m_CurrentShield > m_MaxShield) m_CurrentShield = m_MaxShield;

        return true;
    }

    ///
    /// Private listeners.
    ///

    private void CheckDeath()
    {
        if (m_CurrentHealth <= 0)
        {
            m_CurrentHealth = 0;
            m_IsAttachedCharacterDead = true;
            m_CharacterDeath.Invoke();
        }
    }

    private void UpdateCanvas()
    {
        if (m_CanvasHealthBar == null || m_CanvasShieldBar == null) return;
        float l_HealthPercentile = m_CurrentHealth * 1.0f / m_MaxHealth;
        m_CanvasHealthBar.color = ColorFromGradient(l_HealthPercentile);
        m_CanvasHealthBar.fillAmount = m_CurrentHealth * 1.0f / m_MaxHealth;
        m_CanvasShieldBar.fillAmount = m_CurrentShield * 1.0f / m_MaxShield;
    }

    private void OnCharacterDeath()
    {
        if (m_DisableOnDeath)
        {
            m_AttachedGameObject.SetActive(false);
        }
    }

    private void OnCharacterRespawn()
    {
        m_CurrentHealth = m_StoredHealth;
        m_CurrentShield = m_StoredShield;
        m_IsAttachedCharacterDead = !(m_CurrentHealth > 0);
        if (m_IsAttachedCharacterDead)
        {
            Debug.LogWarning("ERROR While respawning " + this.gameObject.name, this.gameObject);
        }
    }

    public bool IsCharacterDead()
    {
        return m_IsAttachedCharacterDead;
    }

    public void Restart()
    {
        m_RespawnCharacter.Invoke();
    }

    public void UpdateValues()
    {
        m_Activated = this.gameObject.activeSelf;
        m_StoredHealth = m_CurrentHealth;
        m_StoredShield = m_CurrentShield;
    }
}
