using UnityEngine;

public class Credits : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject mainScreen;
    public GameObject creditsPanel;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip buttonClick;

    void Start()
    {
        if(mainScreen == null || creditsPanel == null)
        {
            Debug.LogError("As variáveis 'mainScreen' e 'instructions' precisam ser atribuídas no Inspector.");
            enabled = false;
            return;
        }
        mainScreen.gameObject.SetActive(true);
        creditsPanel.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openCredits()
    {
        if (audioSource != null && buttonClick != null) audioSource.PlayOneShot(buttonClick);
        mainScreen.gameObject.SetActive(false);
        creditsPanel.gameObject.SetActive(true);

    }

    public void backToMainScreen()
    {
        if (audioSource != null && buttonClick != null) audioSource.PlayOneShot(buttonClick);
        mainScreen.gameObject.SetActive(true);
        creditsPanel.gameObject.SetActive(false);
    }
}
