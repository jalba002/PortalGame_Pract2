using UnityEngine;

public interface IRestartable
{
    bool m_Activated { get; set; }
    Vector3 m_InitialPosition { get; set; }
    Quaternion m_InitialRotation { get; set; }
    void Restart();
    void UpdateValues();
}
