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
        var gamepad = Gamepad.current;

        horizontalInput = 0f;
        verticalInput = 0f;

        // Input do Teclado
        if (keyboard != null)
        {
            horizontalInput = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) ? 1f :
                              (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) ? -1f : 0f;
            verticalInput = (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) ? 1f :
                            (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) ? -1f : 0f;
        }
        // Se não houver teclado e nem gamepad, não há input de movimento
        if (keyboard == null && gamepad == null) return;

        // Input do Gamepad (soma-se ao do teclado e depois é limitado)
        if (gamepad != null)
        {
            var leftStick = gamepad.leftStick.ReadValue();
            horizontalInput += leftStick.x;
            verticalInput += leftStick.y;
        }

        // Garante que o input não ultrapasse os limites de -1 a 1
        

        // Verifica o input de tiro do mouse ou do gamepad
        bool shootInput = (gamepad != null && gamepad.rightTrigger.wasPressedThisFrame);

        if (shootInput && Time.time >= nextFireTime)
        {
            Shoot();
        }
    }
}
