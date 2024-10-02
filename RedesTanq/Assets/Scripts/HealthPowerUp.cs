using Photon.Pun;
using UnityEngine;

public class HealthPowerUp : MonoBehaviourPunCallbacks
{
    [SerializeField] private int healthToRestore = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonView playerView = collision.GetComponent<PhotonView>();

            if (playerView != null && playerView.IsMine)
            {
                PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    playerHealth.RestoreHealth(healthToRestore);
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
