using System.Collections;
using Photon.Pun;
using UnityEngine;

public class RapidFirePowerUp : MonoBehaviourPunCallbacks
{
    [SerializeField] private float boostDuration = 6f;
    [SerializeField] private float fireRateMultiplier = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PhotonView playerView = collision.GetComponent<PhotonView>();

            if (playerView != null && playerView.IsMine)
            {
                PlayerController playerController = collision.GetComponent<PlayerController>();

                if (playerController != null)
                {
                    playerController.ApplyRapidFire(fireRateMultiplier, boostDuration);
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
