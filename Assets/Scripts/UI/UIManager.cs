using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public Canvas canvas;
    public RectTransform canvasTransform;
    //public Dictionary<string, RectTransform> ItemsTransforms;
    public GameObject takeResourceSoundPrefab;
    public GameObject pauseMenu;
    public GameObject contextMenu;
    [SerializeField] private GameObject resourceIconPrefab;
    [SerializeField] private RectTransform resourcePanel;
    
    private Dictionary<string, ResourceIcon> resourceIcons = new();

    [Header("Windows")] 
    public BuildingPlacer buildingMenu;
    
    [Header("Debug")]
    public TextMeshProUGUI commandText;
    public TextMeshProUGUI stateText;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one UIManager!");
            Destroy(gameObject);
        }
    }

    #region Resource

    /// <summary>
    /// Обновляет счетчик ресурса. Если иконка еще не создана, она создается.
    /// </summary>
    public void UpdateResourceCount(string resourceName, int amount)
    {
        if (!resourceIcons.ContainsKey(resourceName))
        {
            CreateResourceIcon(resourceName);
        }

        // Обновляем счетчик ресурса
        resourceIcons[resourceName].IncreaseAmount(amount);
    }

    /// <summary>
    /// Создает новую иконку ресурса в UI.
    /// </summary>
    private void CreateResourceIcon(string resourceName)
    {
        // Пример создания иконки для ресурса
        GameObject resourceIcon = Instantiate(resourceIconPrefab, resourcePanel.transform);
        resourceIcon.name = resourceName;
        resourceIcons.Add(resourceName, resourceIcon.GetComponent<ResourceIcon>());

        // Инициализация количества ресурса
        resourceIcon.GetComponent<ResourceIcon>().Initialize();
    }

    
    public RectTransform GetResourceIconTarget(string resourceName)
    {
        if (!resourceIcons.ContainsKey(resourceName))
        {
            CreateResourceIcon(resourceName);
        }

        // Возвращаем RectTransform иконки ресурса
        return resourceIcons[resourceName].GetComponent<RectTransform>();
    }
    
    public ResourceIcon GetResourceIconByName(string resourceName)
    {
        if (!resourceIcons.ContainsKey(resourceName))
        {
            CreateResourceIcon(resourceName);
        }

        // Возвращаем RectTransform иконки ресурса
        return resourceIcons[resourceName].GetComponent<ResourceIcon>();
    }

    public void AddResource(string resourceName)
    {
        if (!resourceIcons.ContainsKey(resourceName))
        {
            CreateResourceIcon(resourceName);
        }

        // Обновляем количество ресурса
        resourceIcons[resourceName].GetComponent<ResourceIcon>().IncreaseAmount(1);
    }
    
    #endregion

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            if (buildingMenu.gameObject.activeSelf)
            {
                buildingMenu.CloseBuildingMenu();
            }
            else
            {
                buildingMenu.OpenBuildingMenu();
            }
        }
    }

    public void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
    }

    public void Pause()
    {
        if (pauseMenu.activeSelf)
        {
            Time.timeScale = 1f;
            pauseMenu.SetActive(false);
        }
        else
        {
            Time.timeScale = 0.00001f;
            pauseMenu.SetActive(true);
        }
    }
}

public class ResourceInventory
{
    public Dictionary<string, int> resources = new Dictionary<string, int>();
}

