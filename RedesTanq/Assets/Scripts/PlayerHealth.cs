using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerHealth : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (!photonView.IsMine) return;

        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // Desactivar el tanque localmente
        gameObject.SetActive(false);

        // Lógica para cuando un jugador muere, como respawnear o finalizar la partida
        Debug.Log("Player died!");

        // Aquí puedes implementar la lógica para respawn o finalizar la partida
        // PhotonNetwork.Destroy(gameObject);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Envía la vida actual a otros jugadores
            stream.SendNext(currentHealth);
        }
        else
        {
            // Recibe la vida actual de otros jugadores
            currentHealth = (int)stream.ReceiveNext();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            // Aplicar daño si es golpeado por una bala
            TakeDamage(1); // Ajusta la cantidad de daño según sea necesario
        }
    }
}
