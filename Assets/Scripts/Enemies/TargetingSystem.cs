using System.Collections.Generic;
using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    [Header("Aiming Configuration")]
    [SerializeField] Camera playerCamera;
    [SerializeField] GameObject highlighterPrefab;
    [SerializeField] RectTransform highlighterCanvas;
    [SerializeField] string targetTag = "Enemy";
    [SerializeField] float maxTargetingDistance = 5000f;

    private List<Target> allTargets = new List<Target>();
    private Dictionary<Target, GameObject> activeHighLighters = new Dictionary<Target, GameObject>();
    void Start()
    {
        if(playerCamera == null) playerCamera = Camera.main;
        FindAllTargets();
    }

    // Update is called once per frame
    void Update()
    {   
        UpdateTargets();
        
    }

    void FindAllTargets()
    {
        allTargets.Clear();
        GameObject[] targetObjects = GameObject.FindGameObjectsWithTag(targetTag);
        foreach(var obj in targetObjects)
        {
            Target target = obj.GetComponent<Target>();
            if(target != null) allTargets.Add(target);
        }
    }

    void UpdateTargets()
    {
        foreach(Target target in allTargets)
        {
            if(target == null || !target.gameObject.activeInHierarchy)
            {
                // remove o destaque se o alvo foi destruído
                if(activeHighLighters.ContainsKey(target))
                {
                    Destroy(activeHighLighters[target]);
                    activeHighLighters.Remove(target);
                }
                continue;
            }

            bool isVisible = isTargetVisible(target.transform);

            if(isVisible)
            {
                if(!activeHighLighters.ContainsKey(target))
                {
                    // Cria um novo destaque para o alvo visível
                    GameObject newHighlighter = Instantiate(highlighterPrefab, highlighterCanvas);
                    activeHighLighters.Add(target, newHighlighter);
                }
                // Atualiza a posição do destaque
                UpdateHighlighterPosition(target, activeHighLighters[target]); 
            }
            else
            {
                // Remove o destaque se o alvo não está mais visível
                if(activeHighLighters.ContainsKey(target))
                {
                    Destroy(activeHighLighters[target]);
                    activeHighLighters.Remove(target);
                }
            }
        }
    }

    bool isTargetVisible(Transform targetTransform)
    {
        // 1. Verifica se está dentro da distância máxima
        float distance = Vector3.Distance(playerCamera.transform.position, targetTransform.position);
        if(distance > maxTargetingDistance)
        {
            return false;
        }

        // 2. Verifica se está dentro do campo de visão da câmera
        Vector3 viewportPoint = playerCamera.WorldToViewportPoint(targetTransform.position);
        if (viewportPoint.z < 0 || viewportPoint.x < 0 || viewportPoint.x > 1 || viewportPoint.y < 0 || viewportPoint.y > 1)
        {
            return false;
        }

        // 3. Verifica se não há obstáculos
        RaycastHit hit;
        Vector3 directionToTarget = (targetTransform.position - playerCamera.transform.position).normalized;
        if(Physics.Raycast(playerCamera.transform.position, directionToTarget, out hit, maxTargetingDistance))
        {
            // Se o raycast atingiu o alvo ele está visível
            if(hit.transform == targetTransform)
            {
                return true;
            }
        }
        return false;
    }

    void UpdateHighlighterPosition(Target target, GameObject highlighter)
    {
        Vector2 screenPosition = playerCamera.WorldToScreenPoint(target.transform.position);
        highlighter.transform.position = screenPosition;
    }
}
