using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class RematchButton : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button rematchButton; // Botón de revancha único para ambos jugadores
    [SerializeField] private GameObject waitingForOpponentText; // Texto para "esperando al oponente"
    [SerializeField] private GameObject player1VictoryScreen; // Referencia a la pantalla de victoria del jugador 1
    [SerializeField] private GameObject player2VictoryScreen; // Referencia a la pantalla de victoria del jugador 2

    private bool playerWantsRematch = false; // Indica si este jugador ha presionado el botón de revancha
    private int playersReadyForRematch = 0; // Cuenta cuántos jugadores han aceptado la revancha
    private VictoryManager victoryManager; // Referencia al VictoryManager para reiniciar la lógica

    private void Start()
    {
        rematchButton.onClick.AddListener(OnRematchButtonClicked);
        waitingForOpponentText.SetActive(false); // Oculta el texto al inicio

        // Obtener la referencia al VictoryManager
        victoryManager = FindObjectOfType<VictoryManager>();
    }

    private void OnRematchButtonClicked()
    {
        playerWantsRematch = true;
        rematchButton.interactable = false; // Desactiva el botón para evitar múltiples clics
        waitingForOpponentText.SetActive(true); // Muestra el texto de "esperando al oponente"
        photonView.RPC("RPC_PlayerWantsRematch", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_PlayerWantsRematch()
    {
        playersReadyForRematch++;

        // Si ambos jugadores han presionado el botón de revancha, inicia la nueva partida
        if (playersReadyForRematch >= 2)
        {
            photonView.RPC("RPC_StartRematch", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_StartRematch()
    {
        // Desactivar pantallas de victoria
        if (player1VictoryScreen != null)
        {
            player1VictoryScreen.SetActive(false);
        }

        if (player2VictoryScreen != null)
        {
            player2VictoryScreen.SetActive(false);
        }

        playerWantsRematch = false;
        playersReadyForRematch = 0;
        rematchButton.interactable = true;
        waitingForOpponentText.SetActive(false);

        // Reiniciar vidas de ambos jugadores
        PlayerHealth[] playerHealths = FindObjectsOfType<PlayerHealth>();
        foreach (var playerHealth in playerHealths)
        {
            if (playerHealth != null)
            {
                playerHealth.RestoreHealth(5);
                playerHealth.gameObject.SetActive(true);
                playerHealth.IsDeath = false;
            }

            // Restablecer posición inicial
            if (playerHealth.isTank1)
            {
                playerHealth.transform.position = new Vector2(-6.322893f, 1.466833f);
            }
            else
            {
                playerHealth.transform.position = new Vector2(8f, 1.396584f);
            }
        }

        // Reactivar el temporizador del juego
        GameTimer gameTimer = FindObjectOfType<GameTimer>();
        if (gameTimer != null)
        {
            gameTimer.ResetTimer(); // Reinicia y comienza el temporizador
        }

        // Reactivar el movimiento de los jugadores (tanques)
        PlayerController[] tankControllers = FindObjectsOfType<PlayerController>();
        foreach (var tankController in tankControllers)
        {
            if (tankController != null)
            {
                tankController.enabled = true; // Reactiva el movimiento del tanque
            }
        }
    }
}
