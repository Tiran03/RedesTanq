using Photon.Pun;
using TMPro;
using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    [SerializeField] private int maxHealth = 100;
    public int currentHealth;
    private bool isInvulnerable = false;
    [SerializeField] private float invulnerabilityDuration = 1f;
    public bool IsDeath = false;

    public TextMeshProUGUI healthText;
    public bool isTank1;

    private VictoryManager victoryManager;

    private void Awake()
    {
        currentHealth = maxHealth;
        victoryManager = FindObjectOfType<VictoryManager>(); // Encontrar el VictoryManager en la escena
    }

    private void Update()
    {
        UpdateHealthUI();
    }

    // Función que sincroniza el daño para todos los jugadores
    public void TakeDamage(int damage)
    {
        photonView.RPC("RPC_TakeDamage", RpcTarget.AllBuffered, damage);
    }

    [PunRPC]
    private void RPC_TakeDamage(int damage)
    {
        if (!isInvulnerable)
        {
            currentHealth -= damage;
            StartCoroutine(InvulnerabilityCoroutine()); // Iniciar invulnerabilidad
            if (currentHealth <= 0)
            {
                IsDeath = true;
                if (IsDeath)
                {
                    Die();
                }
                
            }
        }
        else
        {
            Debug.Log("Player is invulnerable and cannot take damage.");
        }
    }

    // Invulnerabilidad tras recibir daño
    private IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
    }

    // Función que sincroniza la muerte
    private void Die()
    {
        if (IsDeath)
        {
            //gameObject.SetActive(false);
            Debug.Log("Player died!");
            CheckVictory();
        }
        else
        {
            gameObject.SetActive(true);
            currentHealth = 5;
        }
        
    }

    // Actualización de la UI de la salud
    private void UpdateHealthUI()
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {currentHealth}";
        }
        else
        {
            Debug.LogWarning("TextMeshProUGUI reference is missing!");
        }
    }

    // Verificación de la victoria
    private void CheckVictory()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            if (isTank1)
            {
                photonView.RPC("RPC_Player2Wins", RpcTarget.AllBuffered);
            }
            else
            {
                photonView.RPC("RPC_Player1Wins", RpcTarget.AllBuffered);
            }
        }
    }

    [PunRPC]
    private void RPC_Player1Wins()
    {
        if (victoryManager != null)
        {
            victoryManager.photonView.RPC("ShowPlayer1Wins", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void RPC_Player2Wins()
    {
        if (victoryManager != null)
        {
            victoryManager.photonView.RPC("ShowPlayer2Wins", RpcTarget.AllBuffered);
        }
    }

    // Función para aplicar el escudo temporal (invulnerabilidad)
    public void ApplyShield(float shieldDuration)
    {
        photonView.RPC("RPC_ApplyShield", RpcTarget.AllBuffered, shieldDuration);
    }

    [PunRPC]
    private void RPC_ApplyShield(float shieldDuration)
    {
        Debug.Log("Escudo Activo");
        StartCoroutine(ShieldCoroutine(shieldDuration));
    }

    private IEnumerator ShieldCoroutine(float shieldDuration)
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(shieldDuration);
        isInvulnerable = false;
    }

    // Función para restaurar la salud
    public void RestoreHealth(int healthAmount)
    {
        photonView.RPC("RPC_RestoreHealth", RpcTarget.AllBuffered, healthAmount);
    }

    
    //public void RestoreHealth2(int healthToRestore)
    //{
    //    photonView.RPC("RPC_RestoreHealth2", RpcTarget.AllBuffered, healthToRestore);
    //}

    [PunRPC]
    private void RPC_RestoreHealth(int healthAmount)
    {
        Debug.Log("Vida restaurada");
        currentHealth = Mathf.Min(currentHealth + healthAmount, maxHealth);
        UpdateHealthUI();
    }
    
    //[PunRPC]
    //private void RPC_RestoreHealth2(int healthAmount)
    //{
    //    Debug.Log("Vida restaurada2");
    //    currentHealth = maxHealth;
    //    UpdateHealthUI();
    //}
}
