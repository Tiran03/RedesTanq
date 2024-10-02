using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    public float speed = 10f;
    public int damage = 10;
    private bool isPiercing = false;
    private float piercingBulletDuration;
    private float bulletSpeedMultiplier = 1f;

    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!photonView.IsMine) return;

        // Si la bala es perforante, atraviesa al objeto sin destruirse
        if (isPiercing)
        {
            // L�gica para la bala perforante (puedes a�adir m�s comportamiento aqu� si es necesario)
            if (other.CompareTag("Player"))
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
            }
        }
        else
        {
            // L�gica normal para las balas
            if (other.CompareTag("Player"))
            {
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
            }

            PhotonNetwork.Destroy(gameObject); // Destruye la bala si no es perforante
        }
    }

    // M�todo que se llama cuando el poder de bala perforante est� activo
    public void ApplyPiercingEffect(float speedMultiplier, float duration)
    {
        isPiercing = true;
        bulletSpeedMultiplier = speedMultiplier;
        piercingBulletDuration = duration;
        speed *= bulletSpeedMultiplier; // Aumenta la velocidad de la bala

        // Desactiva el efecto despu�s de que termine la duraci�n
        StartCoroutine(DisablePiercingEffectAfterDuration());
    }

    private IEnumerator DisablePiercingEffectAfterDuration()
    {
        yield return new WaitForSeconds(piercingBulletDuration);
        isPiercing = false;
        speed /= bulletSpeedMultiplier; // Restaura la velocidad original
    }
}
