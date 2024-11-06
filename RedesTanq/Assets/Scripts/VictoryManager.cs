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
    private PlayerHealth player1Health; // Vida del jugador 1
    private PlayerHealth player2Health; // Vida del jugador 2
    private Coroutine victoryScreenCoroutine; // Referencia a la corrutina activa
    private bool isRematchInProgress = false; // Indicador de revancha en progreso

    private void Awake()
    {
        player1WinsScreen.SetActive(false);
        player2WinsScreen.SetActive(false);

        // Obtener la referencia al GameTimer
        gameTimer = FindObjectOfType<GameTimer>();
    }

    private void Update()
    {
        // Comprobar y asignar las referencias de vida de los jugadores cuando estén en la sala
        if (player1Health == null || player2Health == null)
        {
            AssignPlayerHealthComponents();
        }
    }

    private void AssignPlayerHealthComponents()
    {
        PlayerHealth[] players = FindObjectsOfType<PlayerHealth>();

        foreach (var player in players)
        {
            if (player.photonView.OwnerActorNr == 1)
            {
                player1Health = player;
            }
            else if (player.photonView.OwnerActorNr == 2)
            {
                player2Health = player;
            }
        }
    }

    // Este método será llamado desde el GameTimer cuando llegue a cero
    public void DetermineWinner()
    {
        if (player1Health == null || player2Health == null)
        {
            Debug.LogWarning("Los componentes de salud no están asignados.");
            return;
        }

        if (player1Health.currentHealth > player2Health.currentHealth)
        {
            photonView.RPC("ShowPlayer1Wins", RpcTarget.All);
        }
        else if (player2Health.currentHealth > player1Health.currentHealth)
        {
            photonView.RPC("ShowPlayer2Wins", RpcTarget.All);
        }
        else
        {
            Debug.Log("Empate!");
        }
    }

    [PunRPC]
    public void ShowPlayer1Wins()
    {
        player1WinsScreen.SetActive(true);
        HandleGameOver();
        StartVictoryScreenTimer();
    }

    [PunRPC]
    public void ShowPlayer2Wins()
    {
        player2WinsScreen.SetActive(true);
        HandleGameOver();
        StartVictoryScreenTimer();
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
                tankController.enabled = false; // Desactiva el movimiento del tanque
            }
        }
    }

    private void StartVictoryScreenTimer()
    {
        StopVictoryScreenCoroutine(); // Asegura detener cualquier corrutina en progreso antes de iniciar una nueva
        isRematchInProgress = false;  // Reestablece el indicador de revancha
        victoryScreenCoroutine = StartCoroutine(VictoryScreenTimer());
    }

    private IEnumerator VictoryScreenTimer()
    {
        float timer = victoryScreenDuration;

        while (timer > 0 && !isRematchInProgress)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        if (timer == 0)
        {
            ReturnToMainMenu();
        }
    }

    private void ReturnToMainMenu()
    {
        PhotonNetwork.LeaveRoom(); // Salir de la sala antes de cargar el menú principal
        SceneManager.LoadScene("MenuScene");
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            int remainingPlayerID = PhotonNetwork.LocalPlayer.ActorNumber;

            if (remainingPlayerID == 1)
            {
                photonView.RPC("ShowPlayer1Wins", RpcTarget.All);
            }
            else if (remainingPlayerID == 2)
            {
                photonView.RPC("ShowPlayer2Wins", RpcTarget.All);
            }
        }
    }

    public void ResetGameForRematch()
    {
        isRematchInProgress = true; // Activar la bandera de revancha

        StopVictoryScreenCoroutine();

        // Ocultar las pantallas de victoria
        player1WinsScreen.SetActive(false);
        player2WinsScreen.SetActive(false);

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

    private void StopVictoryScreenCoroutine()
    {
        if (victoryScreenCoroutine != null)
        {
            StopCoroutine(victoryScreenCoroutine); // Detener la corrutina si está activa
            victoryScreenCoroutine = null; // Limpiar la referencia
        }
    }
}
