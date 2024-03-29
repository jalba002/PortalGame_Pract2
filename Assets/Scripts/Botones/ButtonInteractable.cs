﻿using System.Collections;
using System;
using UnityEngine;

public class ButtonInteractable : InteractableObject
{
    public enum ButtonMode
    {
        Push_Button,
        Temp_Button,
        OnWhileTriggered
    }
    public ButtonMode m_ButtonMode;

    [Header("Configurations")]
    public float m_TempButtonDuration;
    public float m_DelayBetweenActivations;

    private Coroutine m_TempButtonCoroutine;
    private Coroutine m_OnWhileTriggered = null;

    public new void Start()
    {
        base.Start();
    }

    public override bool Interact()
    {
        if (!m_AlreadyInteracting)
        {
            switch (m_ButtonMode)
            {
                case ButtonMode.Push_Button:
                    StartCoroutine(ActivationCooldown());
                    break;
                case ButtonMode.Temp_Button:
                    m_TempButtonCoroutine = StartCoroutine(TempButton());
                    break;
                case ButtonMode.OnWhileTriggered:
                    if (m_OnWhileTriggered != null) StopAllCoroutines();
                    m_OnWhileTriggered = StartCoroutine(OnWhileOn());
                    break;
                default:
                    break;
            }
            return true;
        }
        return false;
    }

    private IEnumerator OnWhileOn()
    {
        m_Activated = true;
        m_AlreadyInteracting = true;
        TurnedOn.Invoke();
        yield return null;
        yield return null;
        Debug.Log("Off");
        m_Activated = false;
        m_AlreadyInteracting = false;
        TurnedOff.Invoke();
        m_OnWhileTriggered = null;
    }

    public IEnumerator TempButton()
    {
        m_Activated = true;
        m_AlreadyInteracting = true;
        TurnedOn.Invoke();
        yield return new WaitForSeconds(m_TempButtonDuration);
        m_Activated = false;
        m_AlreadyInteracting = false;
        TurnedOff.Invoke();
    }

    public void ForceStop()
    {
        m_Activated = false;
        m_AlreadyInteracting = false;
        //StopCoroutine(m_TempButtonCoroutine);
    }

    public IEnumerator ActivationCooldown()
    {
        m_Activated = !m_Activated;
        m_AlreadyInteracting = true;
        if (m_Activated)
        {
            TurnedOn.Invoke();
        }
        else
        {
            TurnedOff.Invoke();
        }
        yield return new WaitForSeconds(m_DelayBetweenActivations);
        m_AlreadyInteracting = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Companion>() != null || other.gameObject == GameController.Instance.GetPlayerGameObject())
        {
            if (Interact())
            {
                //Play some kind of animation for the button
            }
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponent<Companion>() != null || other.gameObject == GameController.Instance.GetPlayerGameObject())
        {
            if (Interact())
            {
                //Play some kind of animation for the button
            }
        }
    }

    public bool IsActivated()
    {
        return m_Activated;
    }
}
