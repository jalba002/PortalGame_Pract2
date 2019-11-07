using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompanionSpawner : MonoBehaviour
{
    public Transform m_SpawnPosition;
    public GameObject m_CompanionPrefab;

    public void SpawnCompanion()
    {
        Instantiate(m_CompanionPrefab, m_SpawnPosition.position, m_SpawnPosition.rotation, null);
    }

}
