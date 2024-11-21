using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class RematchButton : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button rematchButton;
    [SerializeField] private GameObject waitingForOpponentText;
    [SerializeField] private GameObject player1VictoryScreen;
    [SerializeField] private GameObject player2VictoryScreen;
    [SerializeField] private GameObject DrawScreen;

    private bool playerWantsRematch = false;
    private int playersReadyForRematch = 0;
    private VictoryManager victoryManager;

    private void Start()
    {
        rematchButton.onClick.AddListener(OnRematchButtonClicked);
        waitingForOpponentText.SetActive(false);

        
        victoryManager = FindObjectOfType<VictoryManager>();
    }

    private void OnRematchButtonClicked()
    {
        playerWantsRematch = true;
        rematchButton.interactable = false;
        waitingForOpponentText.SetActive(true);
        photonView.RPC("RPC_PlayerWantsRematch", RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_PlayerWantsRematch()
    {
        playersReadyForRematch++;

        
        if (playersReadyForRematch >= 2)
        {
            photonView.RPC("RPC_StartRematch", RpcTarget.All);
        }
    }

    [PunRPC]
    private void RPC_StartRematch()
    {
        
        if (player1VictoryScreen != null)
        {
            player1VictoryScreen.SetActive(false);
        }

        if (player2VictoryScreen != null)
        {
            player2VictoryScreen.SetActive(false);
        }
        
        if (DrawScreen != null)
        {
            DrawScreen.SetActive(false);
        }

        playerWantsRematch = false;
        playersReadyForRematch = 0;
        rematchButton.interactable = true;
        waitingForOpponentText.SetActive(false);

        
        if (victoryManager != null)
        {
            victoryManager.ResetGameForRematch();
        }

        
        PlayerHealth[] playerHealths = FindObjectsOfType<PlayerHealth>();
        foreach (var playerHealth in playerHealths)
        {
            if (playerHealth != null)
            {
                playerHealth.RestoreHealth(5);
                playerHealth.gameObject.SetActive(true);
                playerHealth.IsDeath = false;
            }

            
            if (playerHealth.isTank1)
            {
                playerHealth.transform.position = new Vector2(-6.322893f, 1.466833f);
            }
            else
            {
                playerHealth.transform.position = new Vector2(8f, 1.396584f);
            }
        }

        
        GameTimer gameTimer = FindObjectOfType<GameTimer>();
        if (gameTimer != null)
        {
            gameTimer.ResetTimer();
        }

        
        PlayerController[] tankControllers = FindObjectsOfType<PlayerController>();
        foreach (var tankController in tankControllers)
        {
            if (tankController != null)
            {
                tankController.enabled = true;
            }
        }
    }
}
