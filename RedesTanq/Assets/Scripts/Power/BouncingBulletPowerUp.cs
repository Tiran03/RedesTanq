using System.Collections;
using UnityEngine;
using Photon.Pun;

public class BouncingBulletPowerUp : MonoBehaviourPunCallbacks
{
    [SerializeField] private float bounceDuration = 3f; 

    private AudioSource audioSource;
    [SerializeField] private AudioClip PowerSound; 


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>(); 
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            photonView.RPC("RPC_PlayPowerSound", RpcTarget.All);

            PhotonView playerView = other.GetComponent<PhotonView>();

            
            if (playerView != null && playerView.IsMine)
            {

                
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
        
        SoundManager.Instance.PlaySound("PiercingPowerSound");
    }
}
