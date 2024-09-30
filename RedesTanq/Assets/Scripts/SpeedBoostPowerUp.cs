using System.Collections;
using Photon.Pun;
using UnityEngine;

public class SpeedBoostPowerUp : MonoBehaviourPunCallbacks
{
    [SerializeField] private float boostDuration = 6f; 
    [SerializeField] private float speedMultiplier = 1.5f; 
    [SerializeField] private float rotationMultiplier = 1.5f; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonView playerView = collision.GetComponent<PhotonView>();

            if (playerView != null && playerView.IsMine)
            {
                // Aplicar el poder solo al jugador que lo agarró
                PlayerController playerController = collision.GetComponent<PlayerController>();

                if (playerController != null)
                {
                    playerController.ApplySpeedBoost(speedMultiplier, rotationMultiplier, boostDuration);
                }

                
                photonView.RPC("RPC_DisablePowerUp", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    private void RPC_DisablePowerUp()
    {
        gameObject.SetActive(false);
    }
}
