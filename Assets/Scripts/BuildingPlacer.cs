using UnityEngine;
using UnityEngine.UI;
using Pathfinding;
using TMPro;

public class BuildingPlacer : MonoBehaviour
{
    [Header("Building Settings")]
    public LayerMask walkableLayer;
    public LayerMask buildingLayer; // Слой построенных зданий
    public float buildAreaSize = 3f;
    public GameObject[] buildingPrefabs;

    [Header("UI Settings")]
    public GameObject buttonPrefab;
    public Transform uiPanel;

    private GameObject currentBuilding;
    private GameObject selectedBuilding; // Выбранное здание для удаления или перемещения
    private GridGraph gridGraph;

    private Color originalColor;
    private SpriteRenderer currentBuildingRenderer;

    private void Start()
    {
        gridGraph = AstarPath.active.data.gridGraph;
        GenerateBuildingButtons();
    }

    private void Update()
    {
        HandleBuildingMovement();
        HandleBuildingPlacement();
        HandleBuildingDeselection();
        HandleBuildingInteraction();
    }

    private void HandleBuildingMovement()
    {
        if (currentBuilding == null) return;

        Vector3 mousePosition = GetMouseWorldPosition();
        Vector3 gridPosition = GetSnapToGridPosition(mousePosition);
        currentBuilding.transform.position = gridPosition;
    }

    private Vector3 GetSnapToGridPosition(Vector3 worldPosition)
    {
        int gridX = Mathf.FloorToInt(worldPosition.x / gridGraph.nodeSize);
        int gridY = Mathf.FloorToInt(worldPosition.y / gridGraph.nodeSize);
        return new Vector3(gridX * gridGraph.nodeSize, gridY * gridGraph.nodeSize, 0);
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

    private void HandleBuildingPlacement()
    {
        if (currentBuilding == null) return;
        
        if (CanPlaceBuilding(currentBuilding.transform.position) && !IsPlayerInArea(currentBuilding.transform.position))
        {
            if (currentBuildingRenderer != null)
            {
                originalColor = currentBuildingRenderer.color;
                currentBuildingRenderer.color = Color.green; // Подсветка
            }
        }
        else
        {
        
            if (currentBuildingRenderer != null)
            {
                originalColor = currentBuildingRenderer.color;
                currentBuildingRenderer.color = Color.red; // Подсветка
            }
        }
        
        if (Input.GetMouseButtonDown(0)) // ЛКМ
        {

            
            if (IsPlayerInArea(currentBuilding.transform.position))
            {
                Debug.Log("Невозможно разместить здание, объект игрока находится в зоне строительства.");
                return;
            }

            if (CanPlaceBuilding(currentBuilding.transform.position))
            {
                PlaceBuilding(currentBuilding, currentBuilding.transform.position);
            }
            else
            {
                Debug.Log("Невозможно разместить здание, место занято.");
            }
        }
    }
    
    private bool IsPlayerInArea(Vector3 position)
    {
        // Размер проверки соответствует зоне строительства
        Vector2 size = new Vector2(buildAreaSize * gridGraph.nodeSize, buildAreaSize * gridGraph.nodeSize);
        Collider2D[] hits = Physics2D.OverlapBoxAll(position, size, 0);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player") || hit.CompareTag("Building"))
            {
                return true;
            }
        }

        return false;
    }

    // private void HandleBuildingPlacement()
    // {
    //     if (currentBuilding == null) return;
    //

    //     if (Input.GetMouseButtonDown(0)) // ЛКМ
    //     {
    //         if (CanPlaceBuilding(currentBuilding.transform.position))
    //         {
    //             PlaceBuilding(currentBuilding, currentBuilding.transform.position);
    //         }
    //         else
    //         {
    //             Debug.Log("Невозможно разместить здание, место занято.");
    //         }
    //     }
    // }

    private void HandleBuildingDeselection()
    {
        if (Input.GetMouseButtonDown(1)) // ПКМ
        {
            CancelBuilding();
        }
    }

    private void HandleBuildingInteraction()
    {
        if (Input.GetMouseButtonDown(0) && currentBuilding == null)
        {
            Vector3 mousePosition = GetMouseWorldPosition();
            Collider2D hit = Physics2D.OverlapPoint(mousePosition, buildingLayer);

            if (hit != null)
            {
                selectedBuilding = hit.gameObject;

                // Включаем режим перемещения
                Debug.Log($"Выбрано здание: {selectedBuilding.name}");
                StartBuilding(selectedBuilding);
            }
        }
        else if (Input.GetMouseButtonDown(1) && selectedBuilding != null)
        {
            // Удаляем здание при ПКМ
            RemoveBuilding(selectedBuilding);
            selectedBuilding = null;
        }
    }

    private bool CanPlaceBuilding(Vector3 position)
    {
        int startX = Mathf.FloorToInt((position.x - gridGraph.center.x) / gridGraph.nodeSize) + gridGraph.width / 2;
        int startY = Mathf.FloorToInt((position.y - gridGraph.center.y) / gridGraph.nodeSize) + gridGraph.depth / 2;

        for (int x = startX; x < startX + Mathf.FloorToInt(buildAreaSize); x++)
        {
            for (int y = startY; y < startY + Mathf.FloorToInt(buildAreaSize); y++)
            {
                if (x < 0 || x >= gridGraph.width || y < 0 || y >= gridGraph.depth)
                    return false;

                GridNodeBase nodeBase = gridGraph.GetNode(x, y);
                if (nodeBase == null || !nodeBase.Walkable)
                    return false;
            }
        }

        return true;
    }

    private void PlaceBuilding(GameObject buildingPrefab, Vector3 position)
    {
        GameObject spawnedBuilding = Instantiate(buildingPrefab, position, Quaternion.identity);
        spawnedBuilding.tag = "Building";

        int startX = Mathf.FloorToInt(position.x / gridGraph.nodeSize);
        int startY = Mathf.FloorToInt(position.y / gridGraph.nodeSize);

        for (int x = startX; x < startX + Mathf.FloorToInt(buildAreaSize); x++)
        {
            for (int y = startY; y < startY + Mathf.FloorToInt(buildAreaSize); y++)
            {
                if (x < 0 || x >= gridGraph.width || y < 0 || y >= gridGraph.depth)
                    continue;

                GridNodeBase nodeBase = gridGraph.GetNode(x, y);
                GridNode node = nodeBase as GridNode;
                if (node != null)
                {
                    node.Walkable = false;
                }
            }
        }

        AstarPath.active.Scan();

        Destroy(currentBuilding);
        currentBuilding = null;
    }

    private void StartBuilding(GameObject buildingPrefab)
    {
        if (currentBuilding != null)
        {
            Destroy(currentBuilding);
        }

        currentBuilding = Instantiate(buildingPrefab);
        currentBuildingRenderer = currentBuilding.GetComponent<SpriteRenderer>();

    }

    public void CancelBuilding()
    {
        if (currentBuilding != null)
        {
            if (currentBuildingRenderer != null)
                currentBuildingRenderer.color = originalColor; // Сбрасываем цвет

            Destroy(currentBuilding);
            currentBuilding = null;
        }
    }

    private void RemoveBuilding(GameObject building)
    {
        if (building != null)
        {
            Destroy(building);
            Debug.Log($"Здание {building.name} удалено.");
        }
    }

    private void GenerateBuildingButtons()
    {
        foreach (var prefab in buildingPrefabs)
        {
            GameObject button = Instantiate(buttonPrefab, uiPanel);
            button.GetComponentInChildren<TextMeshProUGUI>().text = prefab.name;

            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                StartBuilding(prefab);
            });
        }
    }

    public void CloseBuildingMenu()
    {
        CancelBuilding();
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }

    public void OpenBuildingMenu()
    {
        gameObject.SetActive(true);
        Time.timeScale = 1f;
        GameEvents.BuildingMenuOpen();
    }
}
