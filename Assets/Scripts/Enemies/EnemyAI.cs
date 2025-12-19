using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class EnemyAI : MonoBehaviour
{
    [Header("Configurações de Movimento")]
    [SerializeField] private float speed = 80f;
    [SerializeField] private float turnSpeed = 2f;
    [SerializeField] private float rollSpeed = 2f;

    [Header("Configurações da IA")]
    [SerializeField] private Transform[] waypoints; // Pontos para a patrulha
    [SerializeField] private float detectionRange = 150f;
    [SerializeField] private float attackRange = 100f;
    [Tooltip("Distância ideal que o inimigo tentará manter do jogador durante o ataque.")]
    [SerializeField] private float maneuverDistance = 80f;
    [Tooltip("Quão impreciso o inimigo é. 0 é mira perfeita, valores maiores aumentam a dispersão.")]
    [SerializeField] private float aimInaccuracy = 1.5f;
    [Tooltip("Com que frequência (em segundos) o inimigo recalcula a mira para o ponto de interceptação.")]
    [SerializeField] private float retargetFrequency = 0.25f;
    [SerializeField] private LayerMask playerLayer; // Para otimizar a detecção

    [Header("Configurações de Tiro")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 500f; // Deve ser igual à velocidade no script do projétil
    [SerializeField] private float fireRate = 2f;

    [Header("Audio")]
    public AudioSource audioSource;
    [SerializeField] private AudioClip shootFX;

    private Rigidbody rb;
    private Transform player;
    private int currentWaypointIndex = 0;
    private float nextFireTime = 0f;
    private float nextRetargetTime = 0f;
    private Vector3 lastPredictedTarget;

    private Rigidbody playerRigidbody;
    // Estados da IA
    private enum AIState { Patrolling, Chasing, Attacking }
    private AIState currentState;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        // Encontra o jogador pela tag "Player". Certifique-se de que sua nave tem essa tag.
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
            playerRigidbody = player.GetComponent<Rigidbody>();
        }
        currentState = AIState.Patrolling;
    }

    void FixedUpdate()
    {
        if (player == null) return; // Se não encontrar o jogador, não faz nada.
        
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        UpdateState(distanceToPlayer);

        switch (currentState)
        {
            case AIState.Patrolling:
                Patrol();
                break;
            case AIState.Chasing:
                Chase();
                break;
            case AIState.Attacking:
                Attack();
                break;
        }
    }

    private void UpdateState(float distanceToPlayer)
    {
        if (distanceToPlayer <= attackRange)
        {
            currentState = AIState.Attacking;
        }
        else if (distanceToPlayer <= detectionRange)
        {
            currentState = AIState.Chasing;
        }
        else
        {
            currentState = AIState.Patrolling;
        }
    }

    private void Patrol()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        MoveTowards(targetWaypoint.position);

        // Se chegar perto do waypoint, vai para o próximo
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 20f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }

    private void Chase()
    {
        MoveTowards(player.position);
    }

    private void Attack()
    {
        // Recalcula a mira em intervalos para um comportamento mais natural
        if (Time.time > nextRetargetTime)
        {
            // Calcula o ponto de interceptação para mirar à frente do jogador
            Vector3 interceptPoint = PredictInterceptPoint(player.position, playerRigidbody.linearVelocity, firePoint.position, projectileSpeed);
            lastPredictedTarget = interceptPoint + (Random.insideUnitSphere * aimInaccuracy); // Adiciona imprecisão
            nextRetargetTime = Time.time + retargetFrequency;
        }
        
        RotateTowards(lastPredictedTarget);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Se estiver mais longe que a distância de manobra, acelera em direção ao jogador.
        // Se estiver muito perto, para de acelerar ou até freia, permitindo que a inércia o afaste um pouco.
        if (distanceToPlayer > maneuverDistance)
        {
            // Aceleração constante para frente
            rb.AddForce(transform.forward * speed, ForceMode.Acceleration);
        }

        // Só atira se estiver mirando na direção do alvo
        Vector3 directionToTarget = (lastPredictedTarget - firePoint.position).normalized;
        float angleToTarget = Vector3.Angle(firePoint.forward, directionToTarget);

        if (Time.time >= nextFireTime && angleToTarget < 5f) // Limita o tiro a um cone de 5 graus
        {
            Shoot();
        }
    }

    /// <summary>
    /// Calcula o ponto futuro onde o projétil e o jogador se encontrarão.
    /// </summary>
    /// <param name="targetPosition">Posição atual do jogador.</param>
    /// <param name="targetVelocity">Velocidade atual do jogador.</param>
    /// <param name="shooterPosition">Posição de onde o tiro sai.</param>
    /// <param name="projectileSpeed">Velocidade do projétil.</param>
    /// <returns>A posição de interceptação.</returns>
    private Vector3 PredictInterceptPoint(Vector3 targetPosition, Vector3 targetVelocity, Vector3 shooterPosition, float projectileSpeed)
    {
        Vector3 displacement = targetPosition - shooterPosition;
        float targetMoveAngle = Vector3.Angle(displacement, targetVelocity);
        
        // Se o alvo está se movendo para longe do atirador, a predição pode ser imprecisa ou impossível.
        // Nesses casos, apenas miramos diretamente.
        if (targetMoveAngle > 90)
        {
            return targetPosition;
        }

        float targetSpeed = targetVelocity.magnitude;
        float a = (targetSpeed * targetSpeed) - (projectileSpeed * projectileSpeed);
        float b = -2 * Vector3.Dot(displacement, targetVelocity);
        float c = displacement.sqrMagnitude;

        float timeToIntercept = (-b - Mathf.Sqrt(b * b - 4 * a * c)) / (2 * a);

        return targetPosition + targetVelocity * timeToIntercept;
    }
    private void MoveTowards(Vector3 targetPosition)
    {
        // Aceleração constante para frente
        rb.AddForce(transform.forward * speed, ForceMode.Acceleration);

        RotateTowards(targetPosition);
    }

    private void RotateTowards(Vector3 targetPosition)
    {
        // Gira suavemente em direção ao alvo
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * turnSpeed));

        // Adiciona um pouco de inclinação (roll) para dar mais realismo
        Vector3 localAngularVelocity = transform.InverseTransformDirection(rb.angularVelocity);
        float rollInput = Mathf.Clamp(-localAngularVelocity.y, -1f, 1f);
        rb.AddTorque(transform.forward * rollInput * rollSpeed);
    }

    void Shoot()
    {
        nextFireTime = Time.time + 1f / fireRate;
        if (shootFX != null)
        {
            AudioSource.PlayClipAtPoint(shootFX, firePoint.position);
        }
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
    }

    // Para visualização dos raios de detecção e ataque no Editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, maneuverDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
