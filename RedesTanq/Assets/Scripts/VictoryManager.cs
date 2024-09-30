using Photon.Pun;
using UnityEngine;

public class VictoryManager : MonoBehaviourPunCallbacks
{
    public GameObject player1WinsImage; // Imagen de victoria del Player 1
    public GameObject player2WinsImage; // Imagen de victoria del Player 2

    private void Awake()
    {
        // Asegúrate de que ambas imágenes estén desactivadas al inicio
        if (player1WinsImage != null && player2WinsImage != null)
        {
            player1WinsImage.SetActive(false);
            player2WinsImage.SetActive(false);
        }
    }

    [PunRPC]
    public void ShowPlayer1Wins()
    {
        player1WinsImage.SetActive(true);
        player2WinsImage.SetActive(false);
    }

    [PunRPC]
    public void ShowPlayer2Wins()
    {
        player1WinsImage.SetActive(false);
        player2WinsImage.SetActive(true);
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
