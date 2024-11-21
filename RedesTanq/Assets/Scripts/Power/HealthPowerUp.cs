using Photon.Pun;
using UnityEngine;

public class HealthPowerUp : MonoBehaviourPunCallbacks
{
    [SerializeField] private int healthToRestore = 1;
    private AudioSource audioSource; 
    [SerializeField] private AudioClip PowerSound; 


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
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
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    private void RPC_PlayPowerSound()
    {
       
        SoundManager.Instance.PlaySound("PiercingPowerSound");
    }
}
