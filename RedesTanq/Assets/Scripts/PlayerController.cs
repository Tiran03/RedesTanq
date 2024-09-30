using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    private PhotonView pv;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 200f;
    [SerializeField] private GameObject bulletPrefab; 
    [SerializeField] private Transform firePoint; 
    [SerializeField] private float fireRate = 0.5f;
    private float nextFireTime = 0f;

    private float originalMoveSpeed;
    private float originalRotationSpeed;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        originalMoveSpeed = moveSpeed;
        originalRotationSpeed = rotationSpeed;
    }

    //private void Start()
    //{
    //    if (pv.IsMine)
    //    {
    //        
    //    }
    //}

    private void Update()
    {
        if (pv.IsMine)
        {
            HandleMovement();
            HandleShooting();
        }
    }

    private void HandleMovement()
    {
        // Rotación del tanque con las teclas A y D
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationSpeed * Time.deltaTime);
        }

        // Movimiento del tanque hacia adelante y atrás con las teclas W y S
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.up * moveSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.position += -transform.up * moveSpeed * Time.deltaTime;
        }
    }

    private void HandleShooting()
    {
        if (Input.GetKey(KeyCode.Space) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            FireBullet();
        }
    }

    private void FireBullet()
    {
        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation);
    }

    // Función para aplicar el poder de velocidad
    public void ApplySpeedBoost(float speedMultiplier, float rotationMultiplier, float boostDuration)
    {
        if (pv.IsMine)
        {
            StartCoroutine(SpeedBoostCoroutine(speedMultiplier, rotationMultiplier, boostDuration));
        }
    }

    private IEnumerator SpeedBoostCoroutine(float speedMultiplier, float rotationMultiplier, float boostDuration)
    {
        // Aumentar la velocidad de movimiento y rotación
        moveSpeed *= speedMultiplier;
        rotationSpeed *= rotationMultiplier;

        // Esperar la duración del boost
        yield return new WaitForSeconds(boostDuration);

        // Restaurar las velocidades originales
        moveSpeed = originalMoveSpeed;
        rotationSpeed = originalRotationSpeed;
    }
}

