using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public float playerHealth = 100f;

    // Variável para guardar a referência ao script PlayerMovement
    private PlayerMovement playerMovement;

    // Você pode declarar uma variável para o Rigidbody se quiser acessá-lo diretamente
    private Rigidbody playerRigidbody;

    // Adicione uma força de rotação para a colisão
    [Header("Configurações de Colisão")]
    [Tooltip("Força de torque aplicada ao Rigidbody da nave na colisão.")]
    public float collisionTorqueForce = 50f;

    [Header("Efeitos de Destruição")]
    [Tooltip("Prefab da explosão a ser instanciado quando o jogador morre.")]
    [SerializeField] private GameObject explosionPrefab;

    [Header("Efeito de Dano na UI")]
    [Tooltip("Referência para a imagem de dano que piscará na tela.")]
    [SerializeField] private Image damageImage;
    [Tooltip("Cor que a tela assumirá ao sofrer dano.")]
    [SerializeField] private Color damageColor = new Color(1f, 0f, 0f, 0.5f); // Vermelho com 50% de alfa
    [Tooltip("Velocidade com que o efeito de dano aparece e desaparece.")]
    [SerializeField] private float flashSpeed = 2f;

    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerRigidbody = playerMovement.rb;
        }
        else
        {
            Debug.LogError("Componente PlayerMovement não encontrado neste GameObject. O Rigidbody não estará acessível para PlayerHealth.");
        }
    }

    void Update()
    {
        
    }

    void OnCollisionEnter(Collision collision)
    {
        if (playerRigidbody != null)
        {
            Vector3 randomTorque = Random.insideUnitSphere.normalized * collisionTorqueForce;
            playerRigidbody.AddTorque(randomTorque, ForceMode.Impulse);
            playerHealth -= 20;
            if (playerHealth <= 0)
            {
                Die();
                return; // Evita que o restante do código seja executado desnecessariamente
            }
            StartCoroutine(DamageColor());
        }
        else
        {
            Debug.LogWarning("Rigidbody não atribuído ao script PlayerHealth, não é possível aplicar torque de colisão.");
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        if(other.CompareTag("EnemyProjectile"))
        {
            playerHealth -= 15;
            if (playerHealth <= 0)
            {
                Die();
                return; // Evita que o restante do código seja executado desnecessariamente
            }
            StartCoroutine(DamageColor());
        }

        if(other.CompareTag("Water")) Die();
    }

    private void Die()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);
    }
    
    /// <summary>
    /// Coroutine que cria um efeito de "flash" vermelho na tela ao sofrer dano.
    /// </summary>
    IEnumerator DamageColor()
    {
        if (damageImage == null) yield break;

        // Aumenta o alfa da cor da imagem para o valor definido em damageColor
        for (float i = 0; i <= damageColor.a; i += Time.deltaTime * flashSpeed)
        {
            damageImage.color = new Color(damageColor.r, damageColor.g, damageColor.b, i);
            yield return null;
        }

        // Diminui o alfa de volta para 0
        for (float i = damageColor.a; i >= 0; i -= Time.deltaTime * flashSpeed)
        {
            damageImage.color = new Color(damageColor.r, damageColor.g, damageColor.b, i);
            yield return null;
        }

        // Garante que a imagem fique totalmente transparente no final
        damageImage.color = new Color(damageColor.r, damageColor.g, damageColor.b, 0);
    }
}
