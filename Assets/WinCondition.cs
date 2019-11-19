using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinCondition : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == GameController.Instance.GetPlayerGameObject())
        {
            GameController.Instance.Win();
        }
    }
}
