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

        
        if (tank1HealthText == null || tank2HealthText == null)
        {
            //Debug.LogError("Health Text references are missing! Please assign them in the Inspector.");
            return;
        }
    }

    void Start()
    {
        Vector2 spawnPosition;

        int playerIndex = PhotonNetwork.PlayerList.Length;

        
        if (playerIndex == 1)
        {
            spawnPosition = new Vector2(-6.322893f, 1.466833f); // Coordenadas de Player 1
        }
        else if (playerIndex == 2)
        {
            spawnPosition = new Vector2(8f, 1.396584f); // Coordenadas de Player 2
        }
        else
        {
            spawnPosition = new Vector2(Random.Range(-4, 4), Random.Range(-4, 4)); // Posición aleatoria en caso de error
        }

      
        player = PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);

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
                playerHealth.healthText = tank1HealthText;
                playerHealth.isTank1 = true;
                //Debug.Log("Assigned Tank1 healthText via RPC.");
            }
            else if (playerIndex == 2)
            {
                playerHealth.healthText = tank2HealthText;
                playerHealth.isTank1 = false;
                //Debug.Log("Assigned Tank2 healthText via RPC.");
            }
        }
    }
}
