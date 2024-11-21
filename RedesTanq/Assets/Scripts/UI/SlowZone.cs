using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowZone : MonoBehaviour
{
    [SerializeField] private float slowDownFactor = 0.5f; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.AdjustSpeed(slowDownFactor); // Ralentiza el tanque
        }
    }


    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            player.ResetSpeed(); // Restablece la velocidad original cuando sale de la zona
        }
    }
}
