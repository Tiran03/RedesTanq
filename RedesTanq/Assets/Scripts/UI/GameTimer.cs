using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameTimer : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private float maxTime = 120f; 
    private float currentTime;
    private bool isTimerStarted = false;

    public TextMeshProUGUI timerText; 
    private bool isGameOver = false;
    private VictoryManager victoryManager; 

    private void Start()
    {
        SetupTimer();
        victoryManager = FindObjectOfType<VictoryManager>();
    }

    private void SetupTimer()
    {
        if (timerText == null)
        {
            GameObject timerTextObject = GameObject.Find("Timer Text");
            if (timerTextObject != null)
            {
                timerText = timerTextObject.GetComponent<TextMeshProUGUI>();
            }
        }

        if (PhotonNetwork.IsMasterClient)
        {
            currentTime = maxTime;
            isTimerStarted = true;
        }
        else
        {
            photonView.RPC("RPC_RequestTimeSync", RpcTarget.MasterClient);
        }
    }

    private void Update()
    {
        if (!isGameOver && isTimerStarted && PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                currentTime -= Time.deltaTime;
                if (currentTime <= 0)
                {
                    currentTime = 0;
                    photonView.RPC("RPC_EndGame", RpcTarget.AllBuffered);
                }
                photonView.RPC("RPC_UpdateTimer", RpcTarget.AllBuffered, currentTime);
            }
            UpdateTimerUI();
        }
    }

    public void StopTimer()
    {
        isTimerStarted = false;
    }

    public void StartTimer()
    {
        isTimerStarted = true;
    }

    public void ResetTimer()
    {
        currentTime = maxTime;
        isGameOver = false;
        isTimerStarted = true;
        UpdateTimerUI();
    }

    [PunRPC]
    private void RPC_UpdateTimer(float time)
    {
        currentTime = time;
        UpdateTimerUI();
    }

    [PunRPC]
    private void RPC_EndGame()
    {
        isGameOver = true;
        if (timerText != null)
        {
            timerText.text = "Game Over!";
        }
        Debug.Log("The game has ended!");

        // Llamar a DetermineWinner() al final del juego
        if (victoryManager != null)
        {
            victoryManager.DetermineWinner();
        }
    }

    [PunRPC]
    private void RPC_RequestTimeSync()
    {
        photonView.RPC("RPC_UpdateTimer", RpcTarget.All, currentTime);
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentTime);
        }
        else
        {
            currentTime = (float)stream.ReceiveNext();
        }
    }
}
