using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [Header("Alvo")]
    public Transform target;

    [Header("Configuração da Câmera")]
    [Tooltip("A posição relativa da câmera em relação à nave")]
    public Vector3 offset = new Vector3(0f, 2.5f, -11f);

    [Tooltip("Quão rápido a câmera se move para a posição do alvo. Valores menores resultam em um movimento mais suave.")]
    [Range(0, 10)]
    public float positionSmoothSpeed = 10f;

    [Tooltip("Quão rápido a câmera gira para acompanhar a rotação do alvo. Valores menores resultam em uma rotação mais suave.")]
    [Range(0, 10)]
    public float rotationSmoothSpeed = 10f;

    void LateUpdate()
    {
        if(!target)
        {
            Debug.LogWarning("A câmera não tem um alvo para seguir. Atribua a nave do jogador ao campo 'Target' no Inspector.");
            return; 
        }
        HandlePosition();
        HandleRotation();
    }

    private void HandlePosition()
    {
        // Calcula a posição desejada da câmera com base no offset em relação à nave
        Vector3 desiredPosition = target.position + target.TransformDirection(offset);

        // Interpola suavemente a posição atual da câmera para a posição desejada
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, positionSmoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    /// <summary>
    /// Lida com o movimento de rotação suave da câmera.
    /// </summary>
    private void HandleRotation()
    {
        // A rotação desejada é a mesma da nave
        Quaternion desiredRotation = target.rotation;

        // Interpola suavemente a rotação atual da câmera para a rotação desejada
        Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotationSmoothSpeed * Time.deltaTime);
        transform.rotation = smoothedRotation;
    }   
}
