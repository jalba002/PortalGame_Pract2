using UnityEngine;

public static class CheckPointManager
{
    public static Checkpoint[] m_AreaCheckPoints = GameController.Instance.m_AreaCheckpoints;
    private static Checkpoint m_CurrentCheckpoint;

    public static bool SetNewCheckpoint(Checkpoint l_Checkpoint)
    {
        for(int i = 0; i < m_AreaCheckPoints.Length; i++)
        {
            if (l_Checkpoint.gameObject == m_AreaCheckPoints[i].gameObject)
            {
                m_CurrentCheckpoint = l_Checkpoint;
                return true;
            }
        }
        return false;
    }

    public static Vector3 GetCurrentCheckPoint()
    {
        return m_CurrentCheckpoint.transform.position;
    }
}
