using UnityEngine;

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
        }
        else
        {
            Debug.LogWarning("Rigidbody não atribuído ao script PlayerHealth, não é possível aplicar torque de colisão.");
        }
    }
}
