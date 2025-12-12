using System.Collections.Generic;
using UnityEngine;

public class TargetingSystem : MonoBehaviour
{
    [Header("Aiming Configuration")]
    [SerializeField] Camera playerCamera;
    [SerializeField] GameObject highlighterPrefab;
    [SerializeField] RectTransform highlighterCanvas;
    [SerializeField] List<string> targetTags = new List<string> { "Enemy", "building" };
    [SerializeField] float maxTargetingDistance = 5000f;

    private List<Target> allTargets = new List<Target>();
    private Dictionary<Target, GameObject> activeHighLighters = new Dictionary<Target, GameObject>();
    private Dictionary<Target, Collider> targetColliders = new Dictionary<Target, Collider>();
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
        targetColliders.Clear();
        foreach (string tag in targetTags)
        {
            GameObject[] targetObjects = GameObject.FindGameObjectsWithTag(tag);
            foreach (var obj in targetObjects)
            {
                Target target = obj.GetComponent<Target>();
                if (target != null) allTargets.Add(target);
                if (target != null && obj.GetComponent<Collider>() != null) targetColliders[target] = obj.GetComponent<Collider>();
            }
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
                    if (targetColliders.ContainsKey(target)) targetColliders.Remove(target);
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

                if (target.CompareTag("building"))
                {
                    UpdateBuildingHighlighter(target, activeHighLighters[target]);
                }
                else
                {
                    // Atualiza a posição do destaque para alvos padrão
                    UpdateHighlighterPosition(target, activeHighLighters[target]);
                }
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
        highlighter.GetComponent<RectTransform>().sizeDelta = new Vector2(100, 100); // Tamanho padrão
    }

    void UpdateBuildingHighlighter(Target target, GameObject highlighter)
    {
        if (!targetColliders.ContainsKey(target)) return;

        Collider targetCollider = targetColliders[target];
        Bounds bounds = targetCollider.bounds;

        // Posição no solo (base do collider)
        Vector3 groundPosition = bounds.center - new Vector3(0, bounds.extents.y+5, 0);
        Vector2 screenPosition = playerCamera.WorldToScreenPoint(groundPosition);
        highlighter.transform.position = screenPosition;

        // Ajusta o tamanho para "circundar" o objeto
        Vector3[] corners = new Vector3[2];
        corners[0] = bounds.center + new Vector3(bounds.extents.x, 0, bounds.extents.z);
        corners[1] = bounds.center - new Vector3(bounds.extents.x, 0, bounds.extents.z);
        float distance = (playerCamera.WorldToScreenPoint(corners[0]) - playerCamera.WorldToScreenPoint(corners[1])).magnitude;
        highlighter.GetComponent<RectTransform>().sizeDelta = new Vector2(distance, distance);
    }
}
