using System.Collections;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
{
    private PhotonView pv;
    public Animator animator;
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

    [SerializeField] private AudioClip shootSound; 
    //[SerializeField] private AudioClip PowerSound; // Clip de sonido de power
    private AudioSource audioSource;

    public static PlayerController localPlayer;
    public bool HasBouncePowerUp { get; private set; } 


    private void Awake()
    {
        pv = GetComponent<PhotonView>();
        originalMoveSpeed = moveSpeed;
        originalRotationSpeed = rotationSpeed;
        originalFireRate = fireRate;
        audioSource = GetComponent<AudioSource>(); 
        if (pv.IsMine)
        {
            localPlayer = this;
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
        
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationSpeed * Time.deltaTime);
        }

        
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
            animator.SetTrigger("Shooting");
            pv.RPC("RPC_PlayShootSound", RpcTarget.All);
            FireBullet();
        }
    }

    private void FireBullet()
    {
        GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, firePoint.position, firePoint.rotation);
        animator.SetTrigger("Shooting");
        
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

       
        yield return new WaitForSeconds(boostDuration);

        
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
        
        fireRate /= fireRateMultiplier;

        
        yield return new WaitForSeconds(boostDuration);

        
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
        
        Debug.Log($"Tank type set to: {currentTankType}");
    }
    public void ActivateBouncePowerUp(float duration)
    {
        StartCoroutine(BouncePowerUpCoroutine(duration));
    }

    private IEnumerator BouncePowerUpCoroutine(float duration)
    {
        HasBouncePowerUp = true; 
        yield return new WaitForSeconds(duration);
        HasBouncePowerUp = false; 
    }

    // Método RPC para reproducir el sonido de disparo
    [PunRPC]
    private void RPC_PlayShootSound()
    {
       
        audioSource.PlayOneShot(shootSound);
    }
    
    
    //[PunRPC]
    //private void RPC_PlayPowerSound()
    //{
    //    // Reproduce el sonido de disparo
    //    audioSource.PlayOneShot(shootSound);
    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("PowerUp"))
    //    {
    //        pv.RPC("RPC_PlayPowerSound", RpcTarget.All);
    //    }

    //}

    
}
