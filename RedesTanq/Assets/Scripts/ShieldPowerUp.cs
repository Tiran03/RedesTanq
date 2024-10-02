using System.Collections;
using Photon.Pun;
using UnityEngine;

public class ShieldPowerUp : MonoBehaviourPunCallbacks
{
    [SerializeField] private float shieldDuration = 5f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonView playerView = collision.GetComponent<PhotonView>();

            if (playerView != null && playerView.IsMine)
            {
                // Aplicar el poder solo al jugador que lo agarró
                PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    playerHealth.ApplyShield(shieldDuration);
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
