using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class RematchButton : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button rematchButton; // Bot�n de revancha �nico para ambos jugadores
    [SerializeField] private GameObject waitingForOpponentText; // Texto para "esperando al oponente"

    private bool playerWantsRematch = false; // Indica si este jugador ha presionado el bot�n de revancha
    private int playersReadyForRematch = 0; // Cuenta cu�ntos jugadores han aceptado la revancha
    // Referencias a las pantallas de victoria
    [SerializeField] private GameObject player1VictoryScreen;
    [SerializeField] private GameObject player2VictoryScreen;

    private void Start()
    {
        rematchButton.onClick.AddListener(OnRematchButtonClicked);
        waitingForOpponentText.SetActive(false); // Oculta el texto al inicio
    }

    private void OnRematchButtonClicked()
    {
        playerWantsRematch = true;
        rematchButton.interactable = false; // Desactiva el bot�n para evitar m�ltiples clics
        waitingForOpponentText.SetActive(true); // Muestra el texto de "esperando al oponente"
        photonView.RPC("RPC_PlayerWantsRematch", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_PlayerWantsRematch()
    {
        playersReadyForRematch++;

        // Si ambos jugadores han presionado el bot�n de revancha, inicia la nueva partida
        if (playersReadyForRematch >= 2)
        {
            RPC_StartRematch();
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
                Debug.Log("Reinicio");
                playerHealth.IsDeath = false;
                playerHealth.RestoreHealth(5);
                playerHealth.gameObject.SetActive(true); // Asegurarse de que ambos jugadores est�n activos

            }
            

            // Restablecer posici�n (ajusta las posiciones seg�n tu l�gica)
            if (playerHealth.isTank1) // Si es el Player 1
            {
                playerHealth.transform.position = new Vector2(-6.322893f, 1.466833f); // Reemplaza con la posici�n inicial del Player 1
            }
            else // Si es el Player 2
            {
                playerHealth.transform.position = new Vector2(8f, 1.396584f); // Reemplaza con la posici�n inicial del Player 2
            }
        }

       
    }

    //public override void OnLeftRoom()
    //{
    //    // Si un jugador se va de la sala, restablece la l�gica de revancha
    //    playerWantsRematch = false;
    //    playersReadyForRematch = 0;
    //    rematchButton.interactable = true;
    //    waitingForOpponentText.SetActive(false);
    //}
}
