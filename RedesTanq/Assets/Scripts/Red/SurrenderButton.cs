using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SurrenderButton : MonoBehaviourPunCallbacks
{
    public void OnSurrenderButtonPressed()
    {
        
        photonView.RPC("RPC_ReturnToMenu", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_ReturnToMenu()
    {
        
        PhotonNetwork.LeaveRoom();

        
        SceneManager.LoadScene("MenuScene");
    }

    
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
