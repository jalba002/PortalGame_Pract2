using UnityEngine;

public class InstaDeath : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        try
        {
            other.gameObject.GetComponent<IDamageable>().DealDamage(999, other);
        }
        catch { }
    }
}
