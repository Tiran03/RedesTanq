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
            photonView.RPC("RPC_PlayPowerSound", RpcTarget.All);

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
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    private void RPC_PlayPowerSound()
    {
        // Llama al SoundManager para reproducir el sonido del poder
        SoundManager.Instance.PlaySound("PiercingPowerSound");
    }
}
