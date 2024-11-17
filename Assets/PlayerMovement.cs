using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    public Transform target; // Ссылка на цель

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Если нажата ЛКМ
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0; // Устанавливаем Z в 0 для 2D
            target.position = mousePosition; // Перемещаем цель
        }
    }
    
    
}
