using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MenuUI : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button createButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMPro.TMP_InputField createInput;
    [SerializeField] private TMPro.TMP_InputField joinInput;
    [SerializeField] private GameObject errorMessage; 

    private void Awake()
    {
        createButton.onClick.AddListener(CreateRoom);
        joinButton.onClick.AddListener(JoinRoom);

        
        errorMessage.SetActive(false);
    }

    private void OnDestroy()
    {
        createButton.onClick.RemoveAllListeners();
        joinButton.onClick.RemoveAllListeners();
    }

    private void CreateRoom()
    {
       
        if (string.IsNullOrEmpty(createInput.text))
        {
            ShowErrorMessage("Debe proporcionar un nombre para crear la sala");
            return; 
        }

        RoomOptions roomConfiguration = new RoomOptions();
        roomConfiguration.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(createInput.text, roomConfiguration);
    }

    private void JoinRoom()
    {
        
        if (string.IsNullOrEmpty(joinInput.text))
        {
            ShowErrorMessage("Debe proporcionar un nombre para unirse a la sala");
            return; 
        }

        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("MapSelection");
    }

    
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (message.Contains("full"))
        {
            ShowErrorMessage("La sala está llena. Intente unirse a otra sala.");
        }
        else
        {
            ShowErrorMessage("No se pudo unir a la sala. Verifique el nombre.");
        }
    }

    // Método para mostrar un mensaje de error
    private void ShowErrorMessage(string errorText)
    {
        if (errorMessage != null)
        {
            
            errorMessage.SetActive(true);
            
            errorMessage.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = errorText;
            
            StartCoroutine(HideErrorMessageAfterDelay(4f));
        }
    }

    
    private IEnumerator HideErrorMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        errorMessage.SetActive(false);
    }
}
