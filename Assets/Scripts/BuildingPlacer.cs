using UnityEngine;
using Pathfinding;

public class BuildingPlacer : MonoBehaviour
{
    public GameObject buildingPrefab;  // Префаб здания
    public LayerMask walkableLayer;    // Маска для проверки проходимости
    public float buildAreaSize = 3f;   // Размер строительной зоны (например, 3x3 клетки)

    private GameObject currentBuilding;  // Текущий строящийся объект
    private GridGraph gridGraph;         // Сетка для навигации

    private void Start()
    {
        gridGraph = AstarPath.active.data.gridGraph;
    }

    private void Update()
    {
        HandleBuildingMovement();
        HandleBuildingPlacement();
    }

    // Перемещение строительного объекта по сетке
    private void HandleBuildingMovement()
    {
        if (currentBuilding == null) return;

        // Получаем позицию мыши на плоскости
        Vector3 mousePosition = GetMouseWorldPosition();

        // Переводим мышиную позицию в координаты сетки
        Vector3 gridPosition = GetSnapToGridPosition(mousePosition);

        // Перемещаем строящийся объект
        currentBuilding.transform.position = gridPosition;
    }

    // Переводим мировые координаты в координаты сетки
    private Vector3 GetSnapToGridPosition(Vector3 worldPosition)
    {
        int gridX = Mathf.FloorToInt(worldPosition.x / gridGraph.nodeSize);
        int gridY = Mathf.FloorToInt(worldPosition.y / gridGraph.nodeSize);
        Vector3 snappedPosition = new Vector3(gridX * gridGraph.nodeSize, gridY * gridGraph.nodeSize, 0);
        return snappedPosition;
    }

    // Получаем мировую позицию мыши
    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        return mousePos;
    }

    // Размещение здания по нажатию ЛКМ
    private void HandleBuildingPlacement()
    {
        if (currentBuilding == null) return;

        if (Input.GetMouseButtonDown(0)) // ЛКМ
        {
            // Проверка, что место для строительства свободно
            if (CanPlaceBuilding(currentBuilding.transform.position))
            {
                // Размещение здания
                PlaceBuilding(currentBuilding.transform.position);
            }
            else
            {
                Debug.Log("Невозможно разместить здание, место занято.");
            }
        }
    }

    private bool CanPlaceBuilding(Vector3 position)
    {
        // Переводим позицию здания в координаты сетки
        int startX = Mathf.FloorToInt((position.x - gridGraph.center.x) / gridGraph.nodeSize) + gridGraph.width / 2;
        int startY = Mathf.FloorToInt((position.y - gridGraph.center.y) / gridGraph.nodeSize) + gridGraph.depth / 2;

        // Проверяем каждую клетку, занимаемую зданием
        for (int x = startX; x < startX + Mathf.FloorToInt(buildAreaSize); x++)
        {
            for (int y = startY; y < startY + Mathf.FloorToInt(buildAreaSize); y++)
            {
                // Проверяем, чтобы координаты оставались в пределах сетки
                if (x < 0 || x >= gridGraph.width || y < 0 || y >= gridGraph.depth)
                    return false; // Вышли за пределы сетки

                // Получаем узел из сетки
                GridNodeBase nodeBase = gridGraph.GetNode(x, y);
                if (nodeBase == null || !nodeBase.Walkable)
                    return false; // Узел недостижим или отсутствует
            }
        }

        return true; // Все клетки доступны и находятся в пределах сетки
    }


    // Размещение здания на выбранной позиции
    private void PlaceBuilding(Vector3 position)
    {
        // Создаем объект на сетке
        Instantiate(buildingPrefab, position, Quaternion.identity);

        // Закрываем клетки, на которых стоит здание
        int startX = Mathf.FloorToInt(position.x / gridGraph.nodeSize);
        int startY = Mathf.FloorToInt(position.y / gridGraph.nodeSize);

        for (int x = startX; x < startX + Mathf.FloorToInt(buildAreaSize); x++)
        {
            for (int y = startY; y < startY + Mathf.FloorToInt(buildAreaSize); y++)
            {
                if (x < 0 || x >= gridGraph.width || y < 0 || y >= gridGraph.depth)
                    continue;

                GridNodeBase nodeBase = gridGraph.GetNode(x, y);

                // Убедитесь, что узел — это GridNode
                GridNode node = nodeBase as GridNode;
                if (node != null)
                {
                    node.Walkable = false; // Закрываем проходимость для этих узлов
                }
            }
        }

        // Пересчитываем граф, чтобы изменения вступили в силу
        AstarPath.active.Scan();

        // Убираем строительный объект (он будет заменен настоящим зданием)
        Destroy(currentBuilding);
        currentBuilding = null; // Убираем ссылку на объект строительства
    }

    // Создание строительного объекта
    public void StartBuilding()
    {
        if (currentBuilding == null)
        {
            currentBuilding = Instantiate(buildingPrefab);  // Создаем объект на экране
        }
    }

    // Отмена строительства
    public void CancelBuilding()
    {
        if (currentBuilding != null)
        {
            Destroy(currentBuilding);
            currentBuilding = null;
        }
    }
}
