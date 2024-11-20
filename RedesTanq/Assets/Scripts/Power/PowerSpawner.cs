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

    // Definir posiciones seguras de spawn
    private List<Vector2> spawnPositions = new List<Vector2>
    {
        new Vector2(-4, 3), new Vector2(0, 1.5f), new Vector2(4, 3),    // Parte superior del mapa
        new Vector2(-3, 0), new Vector2(3, 0),                       // Lados izquierdo y derecho
        new Vector2(-3, -3), new Vector2(0, -1.5f), new Vector2(3, -3)  // Parte inferior del mapa
    };

    private List<Vector2> occupiedPositions = new List<Vector2>(); // Lista de posiciones ocupadas

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

                    // Elegir una posición aleatoria que no esté ocupada
                    Vector2 spawnPosition = GetRandomAvailablePosition();

                    if (spawnPosition != Vector2.zero) // Si hay una posición disponible
                    {
                        // Elegir un poder al azar de la lista
                        int randomPowerIndex = Random.Range(0, powerPrefabs.Count);
                        GameObject randomPower = powerPrefabs[randomPowerIndex];

                        // Instanciar el poder en la posición segura elegida
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

        // Eliminar de la lista las posiciones ya ocupadas
        availablePositions.RemoveAll(pos => occupiedPositions.Contains(pos));

        if (availablePositions.Count > 0)
        {
            // Seleccionar una posición aleatoria entre las disponibles
            int randomIndex = Random.Range(0, availablePositions.Count);
            return availablePositions[randomIndex];
        }

        return Vector2.zero; // No hay posiciones disponibles
    }

    // Método para liberar una posición cuando un poder es recogido o desaparece
    public void FreePosition(Vector2 position)
    {
        occupiedPositions.Remove(position);
    }
}
