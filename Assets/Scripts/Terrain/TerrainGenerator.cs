using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public int width = 100;  // Ширина карты
    public int height = 100; // Высота карты
    public float scale = 20f; // Масштаб шума
    public float threshold = 0.5f;  // Порог для проходимости
    public GameObject grassPrefab;  // Префаб для проходимой земли
    public GameObject rockPrefab;   // Префаб для непроходимой земли

    private void Start()
    {
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        Debug.Log("Prefab Generation");
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Генерация шума с использованием Perlin Noise
                float xCoord = (float)x / width * scale;
                float yCoord = (float)y / height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);

                // Выбор префаба в зависимости от значения шума
                GameObject tilePrefab = sample > threshold ? rockPrefab : grassPrefab;
                Instantiate(tilePrefab, new Vector3(x, 0, y), Quaternion.identity);
            }
        }
    }
}