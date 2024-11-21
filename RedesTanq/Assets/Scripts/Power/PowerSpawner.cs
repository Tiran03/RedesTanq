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

    
    private List<Vector2> spawnPositions = new List<Vector2>
    {
        new Vector2(-4, 3), new Vector2(0, 1.5f), new Vector2(4, 3),    
        new Vector2(-3, 0), new Vector2(3, 0),                       
        new Vector2(-3, -3), new Vector2(0, -1.5f), new Vector2(3, -3)  
    };

    private List<Vector2> occupiedPositions = new List<Vector2>(); 

    private void Start()
    {
        timer = 0;
    }

    private void Update()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount >= 2)
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

                    
                    Vector2 spawnPosition = GetRandomAvailablePosition();

                    if (spawnPosition != Vector2.zero) 
                    {
                        
                        int randomPowerIndex = Random.Range(0, powerPrefabs.Count);
                        GameObject randomPower = powerPrefabs[randomPowerIndex];

                        
                        PhotonNetwork.Instantiate(randomPower.name, spawnPosition, Quaternion.identity);

                        // Agregar la posición a la lista de ocupadas
                        occupiedPositions.Add(spawnPosition);
                    }
                }
            }
        }
    }

    // Método para obtener una posición aleatoria que no esté ocupada
    private Vector2 GetRandomAvailablePosition()
    {
        List<Vector2> availablePositions = new List<Vector2>(spawnPositions);

        
        availablePositions.RemoveAll(pos => occupiedPositions.Contains(pos));

        if (availablePositions.Count > 0)
        {
           
            int randomIndex = Random.Range(0, availablePositions.Count);
            return availablePositions[randomIndex];
        }

        return Vector2.zero; 
    }

    // Método para liberar una posición cuando se agarra un poder 
    public void FreePosition(Vector2 position)
    {
        occupiedPositions.Remove(position);
    }
}
