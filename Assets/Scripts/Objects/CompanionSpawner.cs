using UnityEngine;

public class CompanionSpawner : MonoBehaviour
{
    public Transform m_SpawnPosition;
    public GameObject m_CompanionPrefab;

    public void SpawnCompanion()
    {
        var l_SpawnedObject = Instantiate(m_CompanionPrefab, m_SpawnPosition.position, m_SpawnPosition.rotation, GameController.Instance.m_DestroyInstantiatedObjectsParent);
    }

}
