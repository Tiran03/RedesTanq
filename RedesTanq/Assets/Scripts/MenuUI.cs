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
    [SerializeField] private GameObject roomFullMessage; // Referencia al mensaje de "Sala Llena"

    private void Awake()
    {
        createButton.onClick.AddListener(CreateRoom);
        joinButton.onClick.AddListener(JoinRoom);

        // Asegúrate de que el mensaje esté desactivado al inicio
        roomFullMessage.SetActive(false);
    }

    private void OnDestroy()
    {
        createButton.onClick.RemoveAllListeners();
        joinButton.onClick.RemoveAllListeners();
    }

    private void CreateRoom()
    {
        RoomOptions roomConfiguration = new RoomOptions();
        roomConfiguration.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(createInput.text, roomConfiguration);
    }

    private void JoinRoom()
    {
        PhotonNetwork.JoinRoom(joinInput.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("GamePlay");
    }

    // Método que se llama cuando no se puede unir a la sala
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (message.Contains("full"))
        {
            // Activar el mensaje de "Sala Llena"
            roomFullMessage.SetActive(true);
            // Iniciar corrutina para ocultar el mensaje después de 4 segundos
            StartCoroutine(HideRoomFullMessageAfterDelay(4f));
        }
    }

    // Corrutina para ocultar el mensaje después de un retraso
    private IEnumerator HideRoomFullMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        roomFullMessage.SetActive(false);
    }
}
