using Photon.Pun;
using UnityEngine;

public class PiercingBulletPowerUp : MonoBehaviourPunCallbacks
{
    [SerializeField] private float bulletDuration = 4f;
    [SerializeField] private float bulletSpeedMultiplier = 3f;

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
                    playerController.ApplyPiercingBullet(bulletSpeedMultiplier, bulletDuration);
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
