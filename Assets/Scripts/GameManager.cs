using System.Collections;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] private string buldingFolderTag = "building";
    private int totalbuildings = 0;
    private int destroyedBuildings = 0;

    [SerializeField] private TextMeshProUGUI targestsToDestroyDisplay;
    [SerializeField] private GameObject winningPanel;
    public bool wonTheGame = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        GameObject[] buldings = GameObject.FindGameObjectsWithTag(buldingFolderTag);
        totalbuildings = buldings.Length;
        destroyedBuildings = 0;
        Debug.Log("Registrados " + buldings.Length + " buildings");
    }

    private void Update()
    {
        if (wonTheGame)
        {
            WinTheGame();
            if(Input.GetKeyDown(KeyCode.Return))
            {
                SceneManager.LoadScene(0);
            }
        }
        
        if(targestsToDestroyDisplay != null)
        {
            targestsToDestroyDisplay.text = "TARGETS TO DESTROY: " + (totalbuildings - destroyedBuildings);
        }
    }

    public void NotifyBuldingDestrouyed()
    {
        if (wonTheGame) return;
        destroyedBuildings++;

        if (AllBuldingsDestroyed())
        {
            OnAllBuldingsDestroyed();
        }
    }

    private bool AllBuldingsDestroyed()
    {
        return totalbuildings > 0 && destroyedBuildings >= totalbuildings;
    }

    private void OnAllBuldingsDestroyed()
    {
        wonTheGame = true;

    }

    private void FreezeGame()
    {
        Time.timeScale = 0f;
    }

    private void WinTheGame()
    {

            if (winningPanel != null)
            {
                winningPanel.gameObject.SetActive(true);
            }
            FreezeGame();
            
        
    }

    IEnumerator RestartGame()
    {
        yield return new WaitForSeconds(15f);
        SceneManager.LoadScene(0);
    }
}
