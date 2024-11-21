using Pathfinding;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public int width = 100;  // Ширина карты
    public int height = 100; // Высота карты
    public float mapScale = 20f;
    public int seed = 0;
    public GameObject grassPrefab;  // Префаб для проходимой земли
    public GameObject waterPrefab;   // Префаб для непроходимой земли
    public GridGraph gridGraph;     // Ссылка на граф для Pathfinding
    public Transform parentStatic;
    public Transform parentObstacle;
    
    public int c;

    private void Start()
    {
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
                float xCoord = (float)x / width * mapScale;
                float yCoord = (float)y / height * mapScale;
                float sample = Mathf.PerlinNoise(xCoord + seed, yCoord + seed);
                
                // Генерация объектов (если нужно, для визуализации)
                GameObject tilePrefab = new GameObject();
                Transform parent = parentStatic;
                if (sample <= 0.2f)
                {
                    tilePrefab = waterPrefab;
                    parent = parentObstacle;
                }
                else if (sample is > 0.2f and <= 1f)
                {
                    tilePrefab = grassPrefab;
                    parent = parentStatic;
                }
                
                GameObject spawnedBlock = Instantiate(tilePrefab, new Vector3(x/4f, y/4f, 0) - new Vector3(width/8f, height/8f), Quaternion.identity);
                spawnedBlock.transform.parent = parent;
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
            }
        }

        // Перестроим граф пути
        AstarPath.active.Scan();
    }
}
