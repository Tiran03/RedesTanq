using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPunCallbacks
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifeTime = 5f;

    private void Start()
    {
        // Destruir la bala después de cierto tiempo para evitar que quede flotando
        Destroy(gameObject, lifeTime);
    }

    private void Update()
    {
        // Mover la bala hacia adelante
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine) return;

        // Verificar si la bala ha golpeado a un jugador
        if (collision.CompareTag("Player"))
        {
            // Aquí puedes manejar el daño o cualquier otra lógica
            Debug.Log("Player hit!");

            // Destruir la bala
            PhotonNetwork.Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall"))
        {
            // Destruir la bala si golpea una pared u otro objeto
            PhotonNetwork.Destroy(gameObject);
        }
    }
}
