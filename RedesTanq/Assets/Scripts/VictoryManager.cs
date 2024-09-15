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
        else
        {
            Debug.Log("Victory images are not assigned.");
        }
    }

    [PunRPC]
    public void ShowPlayer1Wins()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            player1WinsImage.SetActive(true);
            player2WinsImage.SetActive(false);
        }
    }

    [PunRPC]
    public void ShowPlayer2Wins()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            player1WinsImage.SetActive(false);
            player2WinsImage.SetActive(true);
        }
    }
}
