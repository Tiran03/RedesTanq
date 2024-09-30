using UnityEngine;
using Photon.Pun;
using TMPro;

public class GameTimer : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField] private float maxTime = 120f; // Tiempo máximo en segundos
    private float currentTime;

    private bool isTimerStarted = false;

    public TextMeshProUGUI timerText; // Referencia al TextMeshPro para mostrar el temporizador
    private bool isGameOver = false;

    private void Start()
    {
        
        if (timerText == null)
        {
            GameObject timerTextObject = GameObject.Find("Timer Text");
            if (timerTextObject != null)
            {
                timerText = timerTextObject.GetComponent<TextMeshProUGUI>();
            }

            if (timerText == null)
            {
                //Debug.LogError("TextMeshProUGUI reference is missing on GameTimer!");
                return;
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
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
        {
            if (!isGameOver && isTimerStarted)
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
            int minutes = Mathf.FloorToInt(currentTime / 60); // Convertir a minutos
            int seconds = Mathf.FloorToInt(currentTime % 60); // Obtener los segundos restantes
            timerText.text = $"{minutes:00}:{seconds:00}"; 
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Enviar el tiempo restante a otros jugadores
            stream.SendNext(currentTime);
        }
        else
        {
            // Recibir el tiempo restante de otros jugadores
            currentTime = (float)stream.ReceiveNext();
        }
    }
}
