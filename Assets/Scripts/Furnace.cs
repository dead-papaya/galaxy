using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class Furnace : Building
{
    public List<ResourceIcon> currentFuel;
    public List<ResourceIcon> currentMaterial;
    public List<ResourceIcon> currentExit;

    public bool isWorking;

    public float currentFuelHealth;
    public float currentFuseCompletion = 0;
    public SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void CheckIfCanWork()
    {
        if (currentFuel.Count > 0 && currentMaterial.Count > 0)
        {
            StartWork();
        }
        else
        {
            StopWork();
        }
    }

    public void StartWork()
    {
        isWorking = true;
        currentFuelHealth = currentFuel[0].fireHealth;
        GetComponent<Animator>().SetBool("isWorking", true);
        if(currentFuelHealth < 0) StopWork();
    }
    
    public void StopWork()
    {
        isWorking = false;
        GetComponent<Animator>().SetBool("isWorking", false);
    }


    private void Update()
    {
        if (isWorking)
        {
            currentFuelHealth -= Time.deltaTime;
            currentFuseCompletion += Time.deltaTime;
            if (currentFuelHealth <= 0)
            {
                currentFuel.Remove(currentFuel[0]);
                
                if (currentFuel.Count == 0)
                {
                    StopWork();
                    return;
                }
                currentFuelHealth = currentFuel[0].fireHealth;
            }

            if (currentFuseCompletion >= 3f)
            {
                currentFuseCompletion = 0f;
                int index = GameManager.Instance.canBeFused.IndexOf(currentMaterial[0].name);
                currentExit.Add(UIManager.Instance.GetResourceIconByName(GameManager.Instance.fused[index]));
                SpawnResource(currentExit[0].currentResourceGO);
                currentExit.Clear();
                currentMaterial.RemoveAt(0);
                if (currentMaterial.Count == 0)
                {
                    StopWork();
                    return;
                }
            }
        }
    }

    private void OnMouseDown()
    {
        if (isPlased)
        {
            OpenMenu();
        }
    }
    
    public override void OpenMenu()
    {
        UIManager.Instance.furnaceWindow.OpenFurnaceWindow(this);       
    }
    
    public void SetFuel(ResourceIcon resourceIcon, int count)
    {
        if (currentFuel.Count > 0)
        {       
            print("GetFuelBack");
            GetFuelBack();
        }
        print("SetFuel");
        for (int i = 0; i < count; i++)
        {
            currentFuel.Add(resourceIcon);
            resourceIcon.IncreaseAmount(-1);
        }
        CheckIfCanWork();
        UIManager.Instance.furnaceWindow.UpdateWindow();
    }

    private void GetFuelBack()
    {
        for (int i = 0; i < currentFuel.Count; i++)
        {
            SpawnResource(currentFuel[i].currentResourceGO);
        }
        currentFuel.Clear();
        CheckIfCanWork();
    }
    
    public void SetMaterial(ResourceIcon resourceIcon, int count)
    {
        if (currentMaterial.Count > 0)
        {       
            print("GetMaterialBack");
            GetMaterialBack();
        }
        print("SetMaterial");
        for (int i = 0; i < count; i++)
        {
            currentMaterial.Add(resourceIcon);
            resourceIcon.IncreaseAmount(-1);
        }
        CheckIfCanWork();
        UIManager.Instance.furnaceWindow.UpdateWindow();
    }
    
    private void GetMaterialBack()
    {
        for (int i = 0; i < currentMaterial.Count; i++)
        {
            SpawnResource(currentMaterial[i].currentResourceGO);
        }
        currentMaterial.Clear();
        CheckIfCanWork();
    }
    private void SpawnResource(GameObject prefab)
    {
        int woodToSpawn = 1; // Количество досок, которые спавнятся при уроне
        for (int i = 0; i < woodToSpawn; i++)
        {
            Vector3 spawnPosition = Vector3.zero;
            bool validPositionFound = false;
            
            spawnPosition = transform.position;

            // Пытаемся найти валидную позицию
            for (int attempt = 0; attempt < 10; attempt++) // Ограничиваем количество попыток
            {
                // Рассчитываем случайное направление
                Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;

                // Рассчитываем потенциальную позицию
                Vector3 potentialPosition =
                    transform.position + (Vector3)randomDirection * UnityEngine.Random.Range(0.5f, 2f);

                spawnPosition = potentialPosition;
                break;
            }

            // Создаем префаб доски
            GameObject wood = Instantiate(prefab, transform.position, Quaternion.identity);

            // Используем DOTween для движения доски
            wood.transform.DOMove(spawnPosition, 0.5f).SetEase(Ease.OutQuad);
        }
    }
}
