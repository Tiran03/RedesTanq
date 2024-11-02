using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MapSelection : MonoBehaviourPunCallbacks
{
    [SerializeField] private Button scenario1Button;
    [SerializeField] private Button scenario2Button;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true; // Asegura que todos los jugadores estén en la misma escena

        if (PhotonNetwork.IsMasterClient)
        {
            scenario1Button.onClick.AddListener(() => SelectScenario(1));
            scenario2Button.onClick.AddListener(() => SelectScenario(2));
        }
        else
        {
            scenario1Button.gameObject.SetActive(false);
            scenario2Button.gameObject.SetActive(false);
        }
    }

    private void SelectScenario(int scenarioIndex)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Guarda el escenario seleccionado como una propiedad de la sala
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { "SelectedScenario", scenarioIndex } });

            // Carga la escena seleccionada
            if (scenarioIndex == 1)
            {
                PhotonNetwork.LoadLevel("Gameplay");
            }
            else if (scenarioIndex == 2)
            {
                PhotonNetwork.LoadLevel("Gameplay2");
            }
        }
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("SelectedScenario", out object selectedScenario))
        {
            int scenarioIndex = (int)selectedScenario;
            if (scenarioIndex == 1)
            {
                PhotonNetwork.LoadLevel("Gameplay");
            }
            else if (scenarioIndex == 2)
            {
                PhotonNetwork.LoadLevel("Gameplay2");
            }
        }
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("SelectedScenario"))
        {
            int scenarioIndex = (int)propertiesThatChanged["SelectedScenario"];
            if (scenarioIndex == 1)
            {
                PhotonNetwork.LoadLevel("Gameplay");
            }
            else if (scenarioIndex == 2)
            {
                PhotonNetwork.LoadLevel("Gameplay2");
            }
        }
    }
}
