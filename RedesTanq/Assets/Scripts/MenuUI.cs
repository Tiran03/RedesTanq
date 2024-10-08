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
    [SerializeField] private GameObject errorMessage; // Referencia al mensaje de "Error"

    private void Awake()
    {
        createButton.onClick.AddListener(CreateRoom);
        joinButton.onClick.AddListener(JoinRoom);

        // Asegúrate de que el mensaje de error esté desactivado al inicio
        errorMessage.SetActive(false);
    }

    private void OnDestroy()
    {
        createButton.onClick.RemoveAllListeners();
        joinButton.onClick.RemoveAllListeners();
    }

    private void CreateRoom()
    {
        // Verificar si el campo de input está vacío
        if (string.IsNullOrEmpty(createInput.text))
        {
            ShowErrorMessage("Debe proporcionar un nombre para crear la sala");
            return; // No continuar si no hay nombre
        }

        RoomOptions roomConfiguration = new RoomOptions();
        roomConfiguration.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(createInput.text, roomConfiguration);
    }

    private void JoinRoom()
    {
        // Verificar si el campo de input está vacío
        if (string.IsNullOrEmpty(joinInput.text))
        {
            ShowErrorMessage("Debe proporcionar un nombre para unirse a la sala");
            return; // No continuar si no hay nombre
        }

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
            // Mostrar el mensaje de error
            errorMessage.SetActive(true);
            // Cambiar el texto del mensaje de error
            errorMessage.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = errorText;
            // Iniciar corrutina para ocultar el mensaje después de 4 segundos
            StartCoroutine(HideErrorMessageAfterDelay(4f));
        }
    }

    // Corrutina para ocultar el mensaje después de un retraso
    private IEnumerator HideErrorMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        errorMessage.SetActive(false); // Ocultar el mensaje
    }
}
