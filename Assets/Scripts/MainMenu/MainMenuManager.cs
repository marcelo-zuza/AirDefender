using UnityEngine;
using UnityEngine.SceneManagement;

// Adicionar este atributo garante que o componente NewGame sempre existirá neste GameObject.
[RequireComponent(typeof(NewGame))]
public class MainMenuManager : MonoBehaviour
{

    [SerializeField] private string sceneName = "Level1";

    private NewGame newGame;

    void Start()
    {
        newGame = GetComponent<NewGame>();
    }

    void Update()
    {
        // Usar "Submit" em vez de KeyCode.Return permite maior flexibilidade
        // com diferentes dispositivos de entrada (teclado, controle, etc.).
        if (newGame.m_InstructionsActive && Input.GetButtonDown("Fire1"))
        {
            // Linha de depuração para confirmar que o código foi executado.
            Debug.Log("Condição atendida! Carregando cena: " + sceneName);
            // É mais seguro carregar a cena pelo nome para evitar problemas
            // se a ordem das cenas for alterada nas Build Settings.
            SceneManager.LoadScene(1);
        }
    }
}
