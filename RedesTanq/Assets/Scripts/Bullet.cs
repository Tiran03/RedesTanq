using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPunCallbacks
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 5f;
    [SerializeField] private int damage = 20; // Cantidad de daño que inflige la bala

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine) return;

        if (collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            PhotonNetwork.Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall"))
        {
            PhotonNetwork.Destroy(gameObject);
        }
    }



}
