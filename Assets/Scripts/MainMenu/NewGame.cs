using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class NewGame : MonoBehaviour
{
    public GameObject mainScreen;
    public GameObject instructions;
    public bool instructionsActive = false;


    void Start()
    {
        mainScreen = GameObject.Find("MainScreen");
        instructions = GameObject.Find("Instructions");
        
        mainScreen.SetActive(true);
        instructions.SetActive(false); 
    }
    public void OpenInstructions()
    {
        mainScreen.SetActive(false);
        instructions.SetActive(true);   
        instructionsActive = true;
    }
    public void StartTheGame()
    {
        if(instructionsActive && Input.GetKeyDown(KeyCode.Return)) SceneManager.LoadScene(1);
    }
}
