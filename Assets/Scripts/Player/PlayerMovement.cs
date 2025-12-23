using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [Header("Configurações de Movimento")]
    [SerializeField] private float speed = 100f;
    [SerializeField] private float turnSpeed = 2f;
    [SerializeField] private float pitchSpeed = 2f;
    [SerializeField] private float rollSpeed = 2f;

    [Header("Limites da Área de Voo")]
    [Tooltip("Raio da área de voo permitida a partir do centro.")]
    [SerializeField] private float flightAreaRadius = 1000f;
    [Tooltip("Centro da área de voo.")]
    [SerializeField] private Vector3 flightAreaCenter = Vector3.zero;
    [Tooltip("Velocidade com que a nave vira para voltar à área de combate.")]
    [SerializeField] private float returnTurnSpeed = 1.5f;


    [Header("Configurações de Tiro")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 15f;
    [SerializeField] private float nextFireTime = 0f;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootFX;

    [Header("UI")]
    [Tooltip("Objeto da UI que contém a mensagem de aviso para voltar à area de combate.")]
    [SerializeField] private GameObject warningMessageObject;

    public Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;
    public TargetingSystem targetingSystem;

    private bool isReturning = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        targetingSystem = GetComponent<TargetingSystem>();

        // Garante que a mensagem de aviso comece desativada
        if (warningMessageObject != null)
        {
            warningMessageObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
    }

    void FixedUpdate()
    {
        float distanceFromCenter = Vector3.Distance(transform.position, flightAreaCenter);

        // Verifica se o jogador saiu da área de combate
        if (distanceFromCenter > flightAreaRadius)
        {
            isReturning = true;
        }
        // Adiciona uma histerese para evitar que o estado fique trocando rapidamente
        else if (isReturning && distanceFromCenter < flightAreaRadius * 0.95f) 
        {
            isReturning = false;
        }

        // Aceleração automática para frente
        rb.AddForce(transform.forward * speed, ForceMode.Acceleration);

        if (isReturning)
        {
            HandleReturnToCombatArea();
            // Ativa a mensagem de aviso se ela existir e não estiver ativa
            if (warningMessageObject != null && !warningMessageObject.activeSelf)
            {
                warningMessageObject.SetActive(true);
            }
        }
        else
        {
            HandlePlayerRotation();
            // Desativa a mensagem de aviso se ela existir e estiver ativa
            if (warningMessageObject != null && warningMessageObject.activeSelf)
            {
                warningMessageObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Controla a rotação da nave com base no input do jogador.
    /// </summary>
    private void HandlePlayerRotation()
    {
        // Movimento de inclinação para cima e para baixo (Pitch)
        rb.AddTorque(transform.right * verticalInput * pitchSpeed);

        // Movimento de virar para esquerda e direita (Yaw)
        rb.AddTorque(transform.up * horizontalInput * turnSpeed);

        // Inclinação lateral ao virar (Roll)
        rb.AddTorque(-transform.forward * horizontalInput * rollSpeed);
    }

    /// <summary>
    /// Controla o retorno automático da nave para a área de combate.
    /// </summary>
    private void HandleReturnToCombatArea()
    {
        // Gira a nave de volta para o centro da area de combate
        Vector3 directionToCenter = (flightAreaCenter - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToCenter);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, returnTurnSpeed * Time.fixedDeltaTime));
    }

    void Shoot()
    {
        nextFireTime = Time.time + 1f / fireRate;
        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        if(audioSource != null && shootFX != null) audioSource.PlayOneShot(shootFX);
    }

    void ReadInput()
    {
        var keyboard = Keyboard.current;
        var gamepad = Gamepad.current;
        var mouse = Mouse.current;

        // Só lê o input de movimento se não estiver no modo de retorno automático
        if (!isReturning)
        {
            horizontalInput = 0f;
            verticalInput = 0f;

            // Input de movimento do Teclado
            if (keyboard != null)
            {
                horizontalInput = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) ? 1f :
                                  (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) ? -1f : 0f;
                verticalInput = (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) ? 1f :
                                (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) ? -1f : 0f;
            }

            // Input do Gamepad (soma-se ao do teclado)
            if (gamepad != null)
            {
                var leftStick = gamepad.leftStick.ReadValue();
                horizontalInput += leftStick.x;
                verticalInput += leftStick.y;
            }

            // Garante que o input não ultrapasse os limites de -1 a 1, como o comentário original sugeria
            horizontalInput = Mathf.Clamp(horizontalInput, -1f, 1f);
            verticalInput = Mathf.Clamp(verticalInput, -1f, 1f);
        }
        else
        {
            // Zera o input de rotação para que o piloto automático assuma
            horizontalInput = 0f;
            verticalInput = 0f;
        }

        // Verifica o input de tiro (disparo contínuo)
        bool shootInput = (mouse != null && mouse.leftButton.wasPressedThisFrame) ||
                          (gamepad != null && gamepad.rightTrigger.wasPressedThisFrame);

        if (shootInput && Time.time >= nextFireTime)
        {
            Shoot();
        }
    }
}
