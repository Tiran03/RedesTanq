using System.Collections;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class VictoryManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject player1WinsScreen;
    [SerializeField] private GameObject player2WinsScreen;
    [SerializeField] private GameObject DrawScreen;
    [SerializeField] private float victoryScreenDuration = 40f; 
    private GameTimer gameTimer; 

    public PlayerController tank1; 
    public PlayerController tank2; 
    private PlayerHealth player1Health; 
    private PlayerHealth player2Health; 
    private Coroutine victoryScreenCoroutine; 
    private bool isRematchInProgress = false; 

    private void Awake()
    {
        player1WinsScreen.SetActive(false);
        player2WinsScreen.SetActive(false);
        DrawScreen.SetActive(false);

        
        gameTimer = FindObjectOfType<GameTimer>();
    }

    private void Update()
    {
        
        if (player1Health == null || player2Health == null)
        {
            AssignPlayerHealthComponents();
        }
    }

    private void AssignPlayerHealthComponents()
    {
        PlayerHealth[] players = FindObjectsOfType<PlayerHealth>();

        foreach (var player in players)
        {
            if (player.photonView.OwnerActorNr == 1)
            {
                player1Health = player;
            }
            else if (player.photonView.OwnerActorNr == 2)
            {
                player2Health = player;
            }
        }
    }

    // Este método se llama desde el GameTimer cuando llegue a cero
    public void DetermineWinner()
    {
        if (player1Health == null || player2Health == null)
        {
            Debug.LogWarning("Los componentes de salud no están asignados.");
            return;
        }

        if (player1Health.currentHealth > player2Health.currentHealth)
        {
            photonView.RPC("ShowPlayer1Wins", RpcTarget.All);
        }
        else if (player2Health.currentHealth > player1Health.currentHealth)
        {
            photonView.RPC("ShowPlayer2Wins", RpcTarget.All);
        }
        else
        {
            Debug.Log("Empate!");
            photonView.RPC("ShowDraw", RpcTarget.All);
        }
    }

    [PunRPC]
    public void ShowPlayer1Wins()
    {
        player1WinsScreen.SetActive(true);
        HandleGameOver();
        StartVictoryScreenTimer();
    }

    [PunRPC]
    public void ShowPlayer2Wins()
    {
        player2WinsScreen.SetActive(true);
        HandleGameOver();
        StartVictoryScreenTimer();
    }
    
    [PunRPC]
    public void ShowDraw()
    {
        DrawScreen.SetActive(true);
        HandleGameOver();
        StartVictoryScreenTimer();
    }

    private void HandleGameOver()
    {
        
        if (gameTimer != null)
        {
            gameTimer.StopTimer();
        }

       
        PlayerController[] tankControllers = FindObjectsOfType<PlayerController>();
        foreach (var tankController in tankControllers)
        {
            if (tankController != null)
            {
                tankController.enabled = false; 
            }
        }
    }

    private void StartVictoryScreenTimer()
    {
        StopVictoryScreenCoroutine(); 
        isRematchInProgress = false; 
        victoryScreenCoroutine = StartCoroutine(VictoryScreenTimer());
    }

    private IEnumerator VictoryScreenTimer()
    {
        float timer = victoryScreenDuration;

        while (timer > 0 && !isRematchInProgress)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        if (timer == 0)
        {
            ReturnToMainMenu();
        }
    }

    private void ReturnToMainMenu()
    {
        PhotonNetwork.LeaveRoom(); 
        SceneManager.LoadScene("MenuScene");
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            int remainingPlayerID = PhotonNetwork.LocalPlayer.ActorNumber;

            if (remainingPlayerID == 1)
            {
                photonView.RPC("ShowPlayer1Wins", RpcTarget.All);
            }
            else if (remainingPlayerID == 2)
            {
                photonView.RPC("ShowPlayer2Wins", RpcTarget.All);
            }
        }
    }

    public void ResetGameForRematch()
    {
        isRematchInProgress = true; 

        StopVictoryScreenCoroutine();

        
        player1WinsScreen.SetActive(false);
        player2WinsScreen.SetActive(false);

       
        if (gameTimer != null)
        {
            gameTimer.ResetTimer();
        }

        // Reactivar la lógica de movimiento de los tanques
        if (tank1 != null)
        {
            tank1.enabled = true; 
        }
        if (tank2 != null)
        {
            tank2.enabled = true;
        }

        // Destruir todos los poderes que hayan quedado en la escena
        GameObject[] remainingPowers = GameObject.FindGameObjectsWithTag("PowerUp");
        foreach (GameObject power in remainingPowers)
        {
            PhotonNetwork.Destroy(power);
        }
    }


    private void StopVictoryScreenCoroutine()
    {
        if (victoryScreenCoroutine != null)
        {
            StopCoroutine(victoryScreenCoroutine);
            victoryScreenCoroutine = null; 
        }
    }
}
