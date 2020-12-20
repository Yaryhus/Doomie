using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathFloor : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {

        FPSCameraAndMovController player = other.transform.GetComponent<FPSCameraAndMovController>();
        if (player != null)
        {
            player.TakeDamage(1000);
        }
    }
}
