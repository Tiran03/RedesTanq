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
            photonView.RPC("RPC_PlayPower6Sound", RpcTarget.All);
            PhotonView playerView = collision.GetComponent<PhotonView>();


            if (playerView != null && playerView.IsMine)
            {
                photonView.RPC("RPC_PlayPower6Sound", RpcTarget.All);
                // Aplicar el poder solo al jugador que lo agarró
                PlayerController playerController = collision.GetComponent<PlayerController>();

                if (playerController != null)
                {
                    playerController.ApplySpeedBoost(speedMultiplier, rotationMultiplier, boostDuration);
                }

                photonView.RPC("RPC_PlayPower6Sound", RpcTarget.All);
                photonView.RPC("RPC_DisablePowerUp", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    private void RPC_DisablePowerUp()
    {
        gameObject.SetActive(false);
    }

    [PunRPC]
    private void RPC_PlayPowerSound()
    {
        // Llama al SoundManager para reproducir el sonido del poder
        SoundManager.Instance.PlaySound("PiercingPowerSound");
    }
}
