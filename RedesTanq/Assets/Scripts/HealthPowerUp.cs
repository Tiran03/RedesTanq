using Photon.Pun;
using UnityEngine;

public class HealthPowerUp : MonoBehaviourPunCallbacks
{
    [SerializeField] private int healthToRestore = 1;
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

    [PunRPC]
    private void RPC_PlayPowerSound()
    {
        // Llama al SoundManager para reproducir el sonido del poder
        SoundManager.Instance.PlaySound("PiercingPowerSound");
    }
}
