using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] GameObject explosionPrefab;
    [Header("Audio")]
    [SerializeField] private AudioClip explosionFX;
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
        PlayExplosionSound();
        if(explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
    
    void PlayExplosionSound()
    {
        if (explosionFX == null) return;

        // Cria um objeto temporário para tocar o som
        GameObject soundObject = new GameObject("ExplosionSound");
        soundObject.transform.position = transform.position;

        // Adiciona um AudioSource e o configura
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = explosionFX;
        audioSource.Play();

        // Destrói o objeto de som após a duração do clipe
        Destroy(soundObject, explosionFX.length);
    }
}
