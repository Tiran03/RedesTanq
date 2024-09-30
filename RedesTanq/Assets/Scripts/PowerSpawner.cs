using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

public class PowerSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> powerPrefabs; // Lista de poderes
    [SerializeField] private float timeToStartSpawning;
    [SerializeField] private float timeBetweenSpawn;

    private bool readyToSpawn;
    private float timer;
    private bool matchStarted = false;

    private void Start()
    {
        timer = 0;
    }

    private void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 1)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                timer += Time.deltaTime;
                matchStarted = true;

                if (!readyToSpawn && timeToStartSpawning < timer)
                {
                    readyToSpawn = true;
                    timer = 0;
                }

                if (readyToSpawn && timer > timeBetweenSpawn)
                {
                    timer = 0;

                    // Elegir un poder al azar de la lista
                    int randomIndex = Random.Range(0, powerPrefabs.Count);
                    GameObject randomPower = powerPrefabs[randomIndex];

                    // Instanciar el poder elegido
                    PhotonNetwork.Instantiate(randomPower.name, new Vector2(Random.Range(-4, 4), Random.Range(-4, 4)), Quaternion.identity);
                }
            }
        }
    }
}

