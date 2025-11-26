using System.Diagnostics;
using UnityEngine;
using UnityEngine.Rendering;

public class Chaser : MonoBehaviour
{
    [SerializeField] float attackRange = 10f;
    [SerializeField] float approachDistance = 5f;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] float fireRate = 1f;
    public GameObject projectilePrefab;
    public Transform firePoint;
    [SerializeField] float repositionRadius = 3f;

    private Transform player;
    private bool isAttacking = false;
    private float lastFireTime;
    private bool isRange = false;
     
    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
