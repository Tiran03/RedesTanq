using Photon.Pun;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviourPunCallbacks
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    public TextMeshProUGUI healthText;
    public bool isTank1; // Determina si es el tanque 1 o 2

    private VictoryManager victoryManager; // Referencia al VictoryManager

    private void Awake()
    {
        currentHealth = maxHealth;
        victoryManager = FindObjectOfType<VictoryManager>(); // Encontrar el VictoryManager en la escena
    }

    private void Update()
    {
        UpdateHealthUI();
    }

    public void TakeDamage(int damage)
    {
        photonView.RPC("RPC_TakeDamage", RpcTarget.AllBuffered, damage);
    }

    [PunRPC]
    private void RPC_TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        gameObject.SetActive(false);
        Debug.Log("Player died!");
        CheckVictory();
    }

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
}
