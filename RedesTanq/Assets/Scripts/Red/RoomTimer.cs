using System.Collections;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomTimer : MonoBehaviourPunCallbacks
{
    private Coroutine roomTimerCoroutine;

    private void Start()
    {
        
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            roomTimerCoroutine = StartCoroutine(RoomTimerCoroutine(30f));
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        
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

        
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel("MenuScene");
        }
    }
}
