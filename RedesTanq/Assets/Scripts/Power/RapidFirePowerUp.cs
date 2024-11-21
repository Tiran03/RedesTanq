using System.Collections;
using Photon.Pun;
using UnityEngine;

public class RapidFirePowerUp : MonoBehaviourPunCallbacks
{
    [SerializeField] private float boostDuration = 6f;
    [SerializeField] private float fireRateMultiplier = 2f;
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
        PhotonNetwork.Destroy(gameObject);
    }


    [PunRPC]
    private void RPC_PlayPowerSound()
    {
        
        SoundManager.Instance.PlaySound("PiercingPowerSound");
    }
}
