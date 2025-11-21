using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [Header("Configurações de Movimento")]
    [SerializeField] private float speed = 100f;
    [SerializeField] private float turnSpeed = 2f;
    [SerializeField] private float pitchSpeed = 2f;
    [SerializeField] private float rollSpeed = 2f;


    [Header("Configurações de Tiro")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 15f;
    [SerializeField] private float nextFireTime = 0f;

    private Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
    }

    void FixedUpdate()
    {
        // Aceleração automática para frente
        rb.AddForce(transform.forward * speed, ForceMode.Acceleration);

        // Movimento de inclinação para cima e para baixo (Pitch)
        rb.AddTorque(transform.right * verticalInput * pitchSpeed);

        // Movimento de virar para esquerda e direita (Yaw)
        rb.AddTorque(transform.up * horizontalInput * turnSpeed);

        // Inclinação lateral ao virar (Roll)
        rb.AddTorque(-transform.forward * horizontalInput * rollSpeed);
    }

    void Shoot()
    {
        nextFireTime = Time.time + 1f / fireRate;
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }

    void ReadInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return; // Nenhum teclado conectado

        // Mapeia WASD para movimento horizontal e vertical
        horizontalInput = 0;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) horizontalInput = 1;
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) horizontalInput = -1;

        verticalInput = 0;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) verticalInput = 1;
        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) verticalInput = -1;

        if(Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextFireTime)
        {
            Shoot();
        }
    }
}
