using System.Collections;
using UnityEngine;

public class BuildingHealth : MonoBehaviour
{
    public float buildingHealth = 100f;
    public GameObject explosionPrefab;

    [Header("Efeitos de Dano")]
    [Tooltip("Cor que o prédio assumirá ao sofrer dano.")]
    [SerializeField] private Color damageColor = Color.yellow;
    [Tooltip("Duração do efeito de piscar a cor de dano.")]
    [SerializeField] private float flashDuration = 0.15f;

    // Referência ao Renderer do objeto para alterar a cor do material.
    private Renderer buildingRenderer;
    // Guarda a cor original do material para restaurá-la após o efeito.
    private Color originalColor;
    // Referência para a corrotina de dano, para podermos pará-la se necessário.
    private Coroutine damageCoroutine;

    void Start()
    {
        // Pega o componente Renderer do prédio.
        buildingRenderer = GetComponent<Renderer>();
        if (buildingRenderer != null)
        {
            // Armazena a cor original do material.
            originalColor = buildingRenderer.material.color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (buildingHealth <= 0)
        {
            Die();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerProjectile"))
        {
            buildingHealth -= 20;

            // Se já houver um efeito de dano em andamento, ele é interrompido para começar um novo.
            if (damageCoroutine != null)
            {
                StopCoroutine(damageCoroutine);
            }
            // Inicia a corrotina que cria o efeito de piscar.
            damageCoroutine = StartCoroutine(DamageFlash());
        }
    }

    void Die()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }

    /// <summary>
    /// Corrotina que cria um efeito de "flash" amarelo no prédio ao sofrer dano.
    /// </summary>
    IEnumerator DamageFlash()
    {
        if (buildingRenderer == null) yield break;

        // Altera a cor do material para a cor de dano.
        buildingRenderer.material.color = damageColor;

        // Espera pela duração definida.
        yield return new WaitForSeconds(flashDuration);

        // Restaura a cor original do material.
        buildingRenderer.material.color = originalColor;

        // Limpa a referência da corrotina.
        damageCoroutine = null;
    }
}
