using System.Collections.Generic;
using UnityEngine;

public class PortalSpawner
{
    private static Vector3 m_Direction;
    private static LayerMask m_PortalLayerMask;

    public static void SpawnPortal(Portal l_PortalToSpawn, RaycastHit l_HitPoint, List<Transform> l_Points)
    {
        m_PortalLayerMask = GameController.Instance.GetPlayerGameObject().GetComponent<PlayerController>().m_EquippedWeapon.m_PortalLayerMask;
        m_Direction = -l_HitPoint.normal;
        if (CheckAllPoints(l_Points))
        {
            l_PortalToSpawn.gameObject.SetActive(true);
            l_PortalToSpawn.SetNewPosition(l_HitPoint);
        }
    }

    private static bool CheckAllPoints(List<Transform> m_ValidPoints)
    {
        for (int i = 0; i < m_ValidPoints.Count; ++i)
        {
            Transform l_ValidPoint = m_ValidPoints[i];
            Ray l_Ray = new Ray(l_ValidPoint.transform.position, m_Direction);
            RaycastHit l_RaycastHit;
            if (Physics.Raycast(l_Ray, out l_RaycastHit, 0.1f, m_PortalLayerMask.value))
            {
                if (l_RaycastHit.collider.gameObject.tag != "PrintableWall")
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            Vector3 l_Direction = (l_ValidPoint.transform.position - m_ValidPoints[0].transform.position);
            Ray l_RayNewRay = new Ray(m_ValidPoints[0].transform.position, l_Direction.normalized);
            Debug.DrawRay(m_ValidPoints[0].transform.position, l_Direction.normalized, Color.red, 1f);
            if (Physics.Raycast(l_RayNewRay, out l_RaycastHit, l_Direction.magnitude, m_PortalLayerMask.value))
            {
                //Debug.Log(i + " returned false");
                return false;
            }
        }
        return true;
    }
}
