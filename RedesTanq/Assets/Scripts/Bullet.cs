using System.Collections;
using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviourPun
{
    public float speed = 10f;
    public int damage = 10;
    private bool isPiercing = false;
    private float piercingBulletDuration;
    private float bulletSpeedMultiplier = 1f;
    private bool isBouncing = false;


    private static bool globalBounceEnabled = false; // Propiedad est�tica para habilitar rebote globalmente

    private void Start()
    {
        if (photonView.IsMine)
        {
            // Activar el efecto de rebote si el jugador que dispar� tiene el poder activo
            isBouncing = PlayerController.localPlayer.HasBouncePowerUp;
        }
    }

    private void Update()
    {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!photonView.IsMine) return;

        // Verificar si el rebote global est� activado y que estamos chocando con una pared
        if (isBouncing && other.CompareTag("Wall"))
        {
            BounceOffWall(other); // Realiza el rebote en la pared
            return;
        }

        // L�gica para balas perforantes
        if (isPiercing)
        {
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

    private void BounceOffWall(Collider2D wall)
    {
        Vector2 normal = (transform.position - (Vector3)wall.ClosestPoint(transform.position)).normalized;
        Vector2 currentDirection = transform.up;
        Vector2 reflectedDirection = Vector2.Reflect(currentDirection, normal);
        transform.up = reflectedDirection;
    }

    // M�todo para activar o desactivar el rebote globalmente
    public static void SetGlobalBounce(bool enabled)
    {
        globalBounceEnabled = enabled;
    }

    // M�todo que se llama cuando el poder de bala perforante est� activo
    public void ApplyPiercingEffect(float speedMultiplier, float duration)
    {
        isPiercing = true;
        bulletSpeedMultiplier = speedMultiplier;
        piercingBulletDuration = duration;
        speed *= bulletSpeedMultiplier; // Aumenta la velocidad de la bala

        StartCoroutine(DisablePiercingEffectAfterDuration());
    }

    private IEnumerator DisablePiercingEffectAfterDuration()
    {
        yield return new WaitForSeconds(piercingBulletDuration);
        isPiercing = false;
        speed /= bulletSpeedMultiplier; // Restaura la velocidad original
    }
}
