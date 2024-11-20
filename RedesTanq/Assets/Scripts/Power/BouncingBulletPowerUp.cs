using System.Collections;
using UnityEngine;
using Photon.Pun;

public class BouncingBulletPowerUp : MonoBehaviourPunCallbacks
{
    [SerializeField] private float bounceDuration = 3f; // Duración del poder de rebote

    private AudioSource audioSource; // Componente AudioSource
    [SerializeField] private AudioClip PowerSound; // Clip de sonido de disparo


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>(); // Obtener el AudioSource
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            photonView.RPC("RPC_PlayPowerSound", RpcTarget.All);

            PhotonView playerView = other.GetComponent<PhotonView>();

            // Asegúrate de que el poder solo afecta al jugador que lo recogió
            if (playerView != null && playerView.IsMine)
            {

                // Activa el efecto de rebote en el jugador que recogió el power-up
                PlayerController playerController = other.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    playerController.ActivateBouncePowerUp(bounceDuration);
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
