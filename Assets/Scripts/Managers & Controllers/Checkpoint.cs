using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Checkpoint : MonoBehaviour
{
    //public bool m_StartingCheckpoint;
    [HideInInspector] public Collider m_Collider;

    private void Awake()
    {
        m_Collider = GetComponent<Collider>();
        m_Collider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider l_Collision)
    {
        if (l_Collision.gameObject == GameController.Instance.GetPlayerGameObject())
        {
            if (CheckPointManager.SetNewCheckpoint(this)) DisableThisCheckpoint();
        }
    }

    private void DisableThisCheckpoint()
    {
        this.m_Collider.enabled = false;
    }
}
