using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject player1WinsScreen;
    [SerializeField] private GameObject player2WinsScreen;
    [SerializeField] private float victoryScreenDuration = 40f; // Duración antes de volver al menú principal

    private void Awake()
    {
        player1WinsScreen.SetActive(false);
        player2WinsScreen.SetActive(false);
    }

    [PunRPC]
    public void ShowPlayer1Wins()
    {
        player1WinsScreen.SetActive(true);
        StartCoroutine(VictoryScreenTimer());
    }

    [PunRPC]
    public void ShowPlayer2Wins()
    {
        player2WinsScreen.SetActive(true);
        StartCoroutine(VictoryScreenTimer());
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
        Debug.Log("Jugador Abandono");

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
}
