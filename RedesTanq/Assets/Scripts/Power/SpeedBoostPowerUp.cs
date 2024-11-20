using System.Collections;
using Photon.Pun;
using UnityEngine;

public class SpeedBoostPowerUp : MonoBehaviourPunCallbacks
{
    [SerializeField] private float boostDuration = 6f; 
    [SerializeField] private float speedMultiplier = 1.5f; 
    [SerializeField] private float rotationMultiplier = 1.5f;
    private AudioSource audioSource; // Componente AudioSource
    [SerializeField] private AudioClip PowerSound; // Clip de sonido de disparo


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // Obtener el AudioSource
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            photonView.RPC("RPC_PlayPowerSound", RpcTarget.All);
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

        PhotonNetwork.Destroy(gameObject);

    }

    [PunRPC]
    private void RPC_PlayPowerSound()
    {
        // Llama al SoundManager para reproducir el sonido del poder
        SoundManager.Instance.PlaySound("PiercingPowerSound");
    }
}
