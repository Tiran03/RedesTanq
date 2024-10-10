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
    private float originalFireRate;

    // Variables para el poder de bala perforante
    private bool isPiercingBulletActive = false;
    private float bulletSpeedMultiplier;
    private float piercingBulletDuration;

    // Define un tipo de tanque
    public enum TankType { Tank1, Tank2 }
    private TankType currentTankType;

    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        originalMoveSpeed = moveSpeed;
        originalRotationSpeed = rotationSpeed;
        originalFireRate = fireRate;
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

        // Si el poder de bala perforante está activo, aplicarlo a la bala
        if (isPiercingBulletActive)
        {
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.ApplyPiercingEffect(bulletSpeedMultiplier, piercingBulletDuration);
            }
        }
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

    // Función para activar la bala perforante
    public void ApplyPiercingBullet(float speedMultiplier, float duration)
    {
        if (pv.IsMine)
        {
            isPiercingBulletActive = true;
            bulletSpeedMultiplier = speedMultiplier;
            piercingBulletDuration = duration;
            StartCoroutine(DeactivatePiercingBulletAfterDuration());
        }
    }

    private IEnumerator DeactivatePiercingBulletAfterDuration()
    {
        yield return new WaitForSeconds(piercingBulletDuration);
        isPiercingBulletActive = false;
    }

    // Función para aplicar el poder de disparo rápido (rapid fire)
    public void ApplyRapidFire(float fireRateMultiplier, float boostDuration)
    {
        if (pv.IsMine)
        {
            StartCoroutine(RapidFireCoroutine(fireRateMultiplier, boostDuration));
        }
    }

    private IEnumerator RapidFireCoroutine(float fireRateMultiplier, float boostDuration)
    {
        // Aumentar la tasa de disparo (disminuye el tiempo entre disparos)
        fireRate /= fireRateMultiplier;

        // Esperar la duración del boost
        yield return new WaitForSeconds(boostDuration);

        // Restaurar la tasa de disparo original
        fireRate = originalFireRate;
    }

    // Método para ralentizar la velocidad cuando el tanque entra en una zona de ralentización
    public void AdjustSpeed(float slowDownFactor)
    {
        moveSpeed *= slowDownFactor;
        rotationSpeed *= slowDownFactor;
    }

    // Método para restaurar la velocidad original cuando el tanque sale de la zona de ralentización
    public void ResetSpeed()
    {
        moveSpeed = originalMoveSpeed;
        rotationSpeed = originalRotationSpeed;
    }

    // Método para establecer el tipo de tanque
    public void SetTankType(TankType tankType)
    {
        currentTankType = tankType;
        // Aquí puedes agregar la lógica para actualizar el modelo del tanque, si es necesario
        Debug.Log($"Tank type set to: {currentTankType}");
    }
}
