using Pathfinding;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public int width = 100;  // Ширина карты
    public int height = 100; // Высота карты
    public float threshold = 0.5f;  // Порог для проходимости
    public GameObject grassPrefab;  // Префаб для проходимой земли
    public GameObject rockPrefab;   // Префаб для непроходимой земли
    public GridGraph gridGraph;     // Ссылка на граф для Pathfinding

    private bool[,] walkableMap;
    public int c;

    private void Start()
    {
        walkableMap = new bool[width, height];
        GenerateTerrain();
        CreateNavMesh();
    }

    void GenerateTerrain()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Генерация шума с использованием Perlin Noise
                float xCoord = (float)x / width;
                float yCoord = (float)y / height;
                float sample = Mathf.PerlinNoise(xCoord * 20, yCoord * 20);

                // Определяем проходимость клетки
                walkableMap[x, y] = sample < threshold;

                // Генерация объектов (если нужно, для визуализации)
                GameObject tilePrefab = walkableMap[x, y] ? grassPrefab : rockPrefab;
                Instantiate(tilePrefab, new Vector3(x, 0, y), Quaternion.identity);
                c++;
            }
        }
    }

    void CreateNavMesh()
    {
        gridGraph = AstarPath.active.data.gridGraph;

        // Инициализация клеток для навигации
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Получаем узел на основе мировых координат
                GridNode node = (GridNode)gridGraph.GetNode(x, y);

                if (node != null)
                {
                    // Настроим проходимость клеток в A* GridGraph
                    node.Walkable = walkableMap[x, y];
                }
            }
        }

        // Перестроим граф пути
        AstarPath.active.Scan();
    }
}
