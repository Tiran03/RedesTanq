using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomTimer : MonoBehaviourPunCallbacks
{
    private Coroutine roomTimerCoroutine;

    private void Start()
    {
        // Solo inicia el temporizador si es el primer jugador en la sala
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            roomTimerCoroutine = StartCoroutine(RoomTimerCoroutine(30f));
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Detiene el temporizador si el segundo jugador entra
        if (roomTimerCoroutine != null && PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            StopCoroutine(roomTimerCoroutine);
            roomTimerCoroutine = null;
        }
    }

    private IEnumerator RoomTimerCoroutine(float countdownTime)
    {
        while (countdownTime > 0)
        {
            yield return new WaitForSeconds(1f);
            countdownTime--;
        }

        // Si el temporizador llega a 0 y el jugador sigue siendo el único en la sala, dejar la sala y regresar al menú
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel("MenuScene");
        }
    }
}
