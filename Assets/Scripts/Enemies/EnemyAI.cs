using UnityEngine;
using System.Collections;

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
    [SerializeField] private LayerMask playerLayer; // Para otimizar a detecção

    [Header("Configurações de Tiro")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireRate = 2f;

    private Rigidbody rb;
    private Transform player;
    private int currentWaypointIndex = 0;
    private float nextFireTime = 0f;

    // Estados da IA
    private enum AIState { Patrolling, Chasing, Attacking }
    private AIState currentState;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // Encontra o jogador pela tag "Player". Certifique-se de que sua nave tem essa tag.
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
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
        MoveTowards(player.position);

        if (Time.time >= nextFireTime)
        {
            Shoot();
        }
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        // Aceleração constante para frente
        rb.AddForce(transform.forward * speed, ForceMode.Acceleration);

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

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
