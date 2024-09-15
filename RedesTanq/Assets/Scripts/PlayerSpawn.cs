using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    private GameObject player;
    private PhotonView pv;

    public TextMeshProUGUI tank1HealthText;
    public TextMeshProUGUI tank2HealthText;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();

        // Verificar que los TextMeshProUGUI estén correctamente asignados en el Inspector
        if (tank1HealthText == null || tank2HealthText == null)
        {
            //Debug.LogError("Health Text references are missing! Please assign them in the Inspector.");
            return;
        }
    }

    void Start()
    {
        player = PhotonNetwork.Instantiate(playerPrefab.name, new Vector2(Random.Range(-4, 4), Random.Range(-4, 4)), Quaternion.identity);
        int playerIndex = PhotonNetwork.PlayerList.Length;

        pv.RPC("AssignHealthText", RpcTarget.AllBuffered, player.GetComponent<PhotonView>().ViewID, playerIndex);

        pv.RPC("ChangeColor", RpcTarget.AllBuffered, player.GetComponent<PhotonView>().ViewID, playerIndex);
    }

    [PunRPC]
    private void ChangeColor(int playerViewID, int playerIndex)
    {
        PhotonView targetPhotonView = PhotonView.Find(playerViewID);

        if (targetPhotonView != null)
        {
            targetPhotonView.gameObject.GetComponent<SpriteRenderer>().color = (playerIndex == 1) ? Color.cyan : Color.yellow;
        }
    }

    [PunRPC]
    private void AssignHealthText(int playerViewID, int playerIndex)
    {
        PhotonView targetPhotonView = PhotonView.Find(playerViewID);

        if (targetPhotonView != null)
        {
            PlayerHealth playerHealth = targetPhotonView.GetComponent<PlayerHealth>();

            // Asignar el TextMeshProUGUI basado en el índice del jugador
            if (playerIndex == 1)
            {
                playerHealth.healthText = tank1HealthText; // Asigna el texto para Tank1
                playerHealth.isTank1 = true;
                //Debug.Log("Assigned Tank1 healthText via RPC.");
            }
            else if (playerIndex == 2)
            {
                playerHealth.healthText = tank2HealthText; // Asigna el texto para Tank2
                playerHealth.isTank1 = false;
                //Debug.Log("Assigned Tank2 healthText via RPC.");
            }
        }
    }
}
