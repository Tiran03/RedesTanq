using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject player1WinsScreen;
    [SerializeField] private GameObject player2WinsScreen;
    [SerializeField] private float victoryScreenDuration = 40f; // Duración antes de volver al menú principal
    private GameTimer gameTimer; // Referencia al GameTimer para detenerlo
   
    public PlayerController tank1; // Referencia al primer tanque
    public PlayerController tank2; // Referencia al segundo tanque


    private void Awake()
    {
        player1WinsScreen.SetActive(false);
        player2WinsScreen.SetActive(false);

        // Obtener la referencia al GameTimer
        gameTimer = FindObjectOfType<GameTimer>();

        // Asumiendo que tienes métodos para asignar los tanques, los obtenemos así:
        //tank1 = GameObject.Find("Tank1").GetComponent<PlayerController>();
        //tank2 = GameObject.Find("Tank2").GetComponent<PlayerController>();
    }

    [PunRPC]
    public void ShowPlayer1Wins()
    {
        player1WinsScreen.SetActive(true);
        HandleGameOver();
        StartCoroutine(VictoryScreenTimer());
    }

    [PunRPC]
    public void ShowPlayer2Wins()
    {
        player2WinsScreen.SetActive(true);
        HandleGameOver();
        StartCoroutine(VictoryScreenTimer());
    }

    private void HandleGameOver()
    {
        // Detener el GameTimer
        if (gameTimer != null)
        {
            gameTimer.StopTimer();
        }

        // Desactivar el movimiento de los jugadores (tanques)
        PlayerController[] tankControllers = FindObjectsOfType<PlayerController>();
        foreach (var tankController in tankControllers)
        {
            if (tankController != null)
            {
                //Debug.Log("Desactivando Tanque");
                tankController.enabled = false; // Desactiva el movimiento del tanque
            }
        }
    }

    private IEnumerator VictoryScreenTimer()
    {
        float timer = victoryScreenDuration;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        // Cuando el tiempo llega a 0, se vuelve al menú principal
        //ReturnToMainMenu();
    }

    private void ReturnToMainMenu()
    {
        PhotonNetwork.LeaveRoom(); // Salir de la sala antes de cargar el menú principal
        SceneManager.LoadScene("MenuScene");
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("Jugador Abandonó");

        // Verificar el PhotonView ID del jugador que queda
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            int remainingPlayerID = PhotonNetwork.LocalPlayer.ActorNumber;

            if (remainingPlayerID == 1)
            {
                Debug.Log("Gana jugador 1");
                photonView.RPC("ShowPlayer1Wins", RpcTarget.All);
            }
            else if (remainingPlayerID == 2)
            {
                Debug.Log("Gana jugador 2");
                photonView.RPC("ShowPlayer2Wins", RpcTarget.All);
            }
        }
    }

    public void ResetGameForRematch()
    {
        // Reactivar el GameTimer
        if (gameTimer != null)
        {
            gameTimer.ResetTimer(); // Reiniciar el temporizador del juego
        }

        // Reactivar la lógica de movimiento de los tanques
        if (tank1 != null)
        {
            tank1.enabled = true; // Permitir el movimiento del tanque 1
        }
        if (tank2 != null)
        {
            tank2.enabled = true; // Permitir el movimiento del tanque 2
        }
    }
}
