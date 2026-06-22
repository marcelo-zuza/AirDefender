using UnityEngine;

public class BuildingDestructor : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        if (GameManager.instance != null) GameManager.instance.NotifyBuldingDestrouyed();
    }
}
