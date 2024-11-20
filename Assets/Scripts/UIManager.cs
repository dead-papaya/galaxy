using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    
    public static UIManager Instance { get; private set; }
    
    private Camera mainCamera;
    [SerializeField] private RectTransform canvasRectTransform;  // Ссылка на RectTransform Canvas
    [SerializeField] private RectTransform menuRectTransform; 
    
    public GameObject menuPanel; // Панель контекстного меню
    public Button buttonPrefab;  // Префаб кнопки

    public TextMeshProUGUI treeResourceCountText;
    public TextMeshProUGUI currentStateTMPro;
    public TextMeshProUGUI currentCommandListLengthTMPro;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void GenerateMenu(Vector3 position, List<Command> commands)
    {
        // Очищаем старые кнопки
        foreach (Transform child in menuPanel.transform)
        {
            Destroy(child.gameObject);
        }
        
        if (commands == null || commands.Count == 0)
        {
            Debug.LogError("Список команд пуст или не передан в GenerateMenu!");
            return;
        }
        
        // Создаём кнопки для каждой команды
        foreach (Command command in commands)
        {
            Button newButton = Instantiate(buttonPrefab, menuPanel.transform);
            newButton.GetComponentInChildren<TextMeshProUGUI>().text = command.commandName;

            // Привязываем действие к кнопке
            newButton.onClick.AddListener(delegate
            {
                command.bear = BearManager.Instance.GetSelectedBear();
                BearManager.Instance.GetSelectedBear().AddCommand(command);
                CloseMenu();
            });
        }
        
        

        // Показываем меню в позиции курсора
        menuPanel.transform.position = ClampToScreen(position);
        menuPanel.SetActive(true);
    }

    public void CloseMenu()
    {
        menuPanel.SetActive(false);
    }

    private void Start()
    {
        mainCamera = Camera.main;
        menuPanel.SetActive(false); // Прячем меню по умолчанию
    }

    void Update()
    {
        if (BearManager.Instance.GetSelectedBear() != null)
        {
            if (Input.GetMouseButtonDown(1)) // ПКМ
            {
                Vector3 mousePosition = Input.mousePosition;

                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mousePosition), Vector2.zero);

                GameObject clickedObject = hit.collider != null ? hit.collider.gameObject : null;
                CommandList commandList;
                if (clickedObject != null && clickedObject.TryGetComponent<CommandList>(out commandList))
                {
                    print("clickedObject != null && clickedObject.TryGetComponent<CommandList>(out commandList)");
                    GenerateMenu(ClampToScreen(mousePosition), commandList.Commands);
                }
                else
                {
                    GenerateMenu(ClampToScreen(mousePosition), new List<Command>(){new MoveCommand(BearManager.Instance.GetSelectedBear(),mainCamera.ScreenToWorldPoint(mousePosition))});
                }
            }
        }
    }
    
    /// <summary>
    /// Ограничивает позицию меню в пределах экрана и размещает справа снизу от мыши.
    /// </summary>
    private Vector3 ClampToScreen(Vector3 position)
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Учитываем размеры меню
        float menuWidth = menuRectTransform.rect.width;
        float menuHeight = menuRectTransform.rect.height;

        // Смещение: размещение справа снизу от мыши
        position.x += menuWidth / 4f;  // Сдвигаем вправо
        position.y -= menuHeight / 4f; // Сдвигаем вниз

        // Ограничение по горизонтали
        if (position.x < 0)
            position.x = 0;
        if (position.x + menuWidth > screenWidth)
            position.x = screenWidth - menuWidth;

        // Ограничение по вертикали
        if (position.y < 0)
            position.y = 0;
        if (position.y + menuHeight > screenHeight)
            position.y = screenHeight - menuHeight;

        return position;
    }

}
