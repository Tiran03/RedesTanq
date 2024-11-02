using System.Collections;
using UnityEngine;
using Photon.Pun;

public class BouncingBulletPowerUp : MonoBehaviourPunCallbacks
{
    [SerializeField] private float bounceDuration = 3f; // Duración del poder de rebote

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
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

                photonView.RPC("RPC_DisablePowerUp", RpcTarget.AllBuffered); // Destruye el power-up en la escena
            }
        }
    }

    [PunRPC]
    private void RPC_DisablePowerUp()
    {
        gameObject.SetActive(false);
    }
}
