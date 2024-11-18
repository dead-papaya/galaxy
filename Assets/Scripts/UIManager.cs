using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Camera mainCamera;
    [SerializeField] private GameObject contextMenu;
    [SerializeField] private RectTransform canvasRectTransform;  // Ссылка на RectTransform Canvas
    [SerializeField] private RectTransform menuRectTransform;  

    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (BearManager.Instance.selectedBear != null)
        {
            if (Input.GetMouseButtonDown(1)) // ПКМ
            {
                // Позиция мыши в экранных координатах
                Vector3 mousePos = Input.mousePosition;

                // Устанавливаем позицию меню справа снизу
                Vector3 adjustedPos = mousePos;
                adjustedPos.x += 75; // Смещение вправо (добавьте отступ, если нужно)
                adjustedPos.y -= 50; // Смещение вниз

                // Ограничиваем позицию, чтобы меню оставалось в пределах экрана
                adjustedPos = ClampToScreen(adjustedPos);

                // Устанавливаем позицию меню
                contextMenu.transform.position = adjustedPos;

                // Показываем меню
                contextMenu.SetActive(true);
            }
        }
    }
    
    /// <summary>
    /// Ограничивает позицию меню в пределах экрана
    /// </summary>
    private Vector3 ClampToScreen(Vector3 position)
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Учитываем размеры меню
        float menuWidth = menuRectTransform.rect.width/2f;
        float menuHeight = menuRectTransform.rect.height/2f;

        // Ограничение по горизонтали
        if (position.x < 0)
            position.x = 0;
        if (position.x + menuWidth > screenWidth)
            position.x = screenWidth - menuWidth;

        // Ограничение по вертикали
        if (position.y < menuHeight)
            position.y = menuHeight;
        if (position.y > screenHeight)
            position.y = screenHeight;

        return position;
    }
}
