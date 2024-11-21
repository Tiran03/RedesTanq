using System.Collections;
using UnityEngine;
using Photon.Pun;

public class GameStartManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private float startDelay = 3f; // Tiempo de espera antes de permitir el movimiento
    private bool bothPlayersReady = false;
    private PlayerController[] playerControllers;


    private void Start()
    {
        playerControllers = FindObjectsOfType<PlayerController>();
        TogglePlayerMovement(false); // Desactiva el movimiento al inicio
        StartCoroutine(CheckPlayersReady());
    }

    private IEnumerator CheckPlayersReady()
    {
        
        yield return new WaitUntil(() => PhotonNetwork.CurrentRoom.PlayerCount >= 2);

        bothPlayersReady = true; 
        yield return new WaitForSeconds(startDelay); // Espera los 3 segundos

        TogglePlayerMovement(true); // Permite el movimiento después de esperar
    }

    private void TogglePlayerMovement(bool canMove)
    {
        foreach (var playerController in playerControllers)
        {
            if (playerController != null)
            {
                playerController.enabled = canMove;
            }
        }
    }
}
