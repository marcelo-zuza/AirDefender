using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour
{
    [Tooltip("A tela principal do menu.")]
    public GameObject mainScreen;
    [Tooltip("A tela de instruções.")]
    public GameObject instructions;
    [Tooltip("O nome da cena do jogo a ser carregada.")]
    public string gameSceneName = "Level1"; // Ou o nome da sua cena

    public bool m_InstructionsActive = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClick;

    private void Start()
    {
        // É recomendado atribuir 'mainScreen' e 'instructions' pelo Inspector da Unity.
        // Se eles não forem atribuídos, desativamos o script para evitar erros.
        if (mainScreen == null || instructions == null)
        {
            Debug.LogError("As variáveis 'mainScreen' e 'instructions' precisam ser atribuídas no Inspector.");
            enabled = false;
            return;
        }

        mainScreen.gameObject.SetActive(true);
        instructions.gameObject.SetActive(false);
    }

    private void Update()
    {

    }

    public void OpenInstructions()
    {
        if(audioSource != null & buttonClick != null) audioSource.PlayOneShot(buttonClick);
        mainScreen.gameObject.SetActive(false);
        instructions.gameObject.SetActive(true);
        m_InstructionsActive = true;
    }

    public void StartGame()
    {
        if(audioSource != null & buttonClick != null) audioSource.PlayOneShot(buttonClick);
        SceneManager.LoadScene(1);
    }
}
