using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootFx;
    public AudioClip explosionFX;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PlayerProjectile"))
        {
            Destroy(other.gameObject);
            Die();
            return;
        }
    }

    void Die()
    {
        if(explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }


}
