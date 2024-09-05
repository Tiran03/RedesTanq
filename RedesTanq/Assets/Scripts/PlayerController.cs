using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    private PhotonView pv;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 200f;
    //private Camera camera;
    [SerializeField] private GameObject bulletPrefab; // Prefab de la bala
    [SerializeField] private Transform firePoint; // Punto desde donde se disparará la bala
    [SerializeField] private float fireRate = 0.5f;
    private float nextFireTime = 0f;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        //camera = GetComponentInChildren<Camera>();
    }

    private void Start()
    {
        if (pv.IsMine)
        {
            //camera.gameObject.SetActive(pv.IsMine);
        }
    }

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
        if (pv.IsMine)
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
}

