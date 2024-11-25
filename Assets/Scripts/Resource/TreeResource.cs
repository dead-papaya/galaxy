using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class TreeResource : ResourceObject
{
    private void Awake()
    {
        GetComponent<CommandList>().Commands = new List<Command>(){(new HarvestResourceCommand(null, this))};
        if (Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y), new Vector2(2f, 2f), 0f, playerLayerMask))
        {
            print("DESTROY TREE AT POS: " + transform.position);
            Destroy(gameObject);
        }
    }

    // public override async void TakeDamage()
    // {
    //     health--;
    //     Debug.Log($"Дерево {name} получило урон. Осталось здоровья: {health}");
    //     //Spawn resource
    //     //UIManager.Instance.woodText.text = (Convert.ToInt32(UIManager.Instance.woodText.text) + 1).ToString();
    //     //Добавить дерево игроку
    //     GameObject spawnedSound = Instantiate(harvestSound);
    //     if (health <= 0)
    //     {
    //         GameObject spawnedEndSound = Instantiate(harvestEndSound);
    //         Destroy(spawnedEndSound, 3f);
    //     }
    //     
    //     await ShakeTree();
    //     Destroy(spawnedSound);
    //
    //     if (health <= 0)
    //     {
    //         Deplete();
    //     }
    // }
    
    public override async void TakeDamage()
    {
        health--;
        Debug.Log($"Дерево {name} получило урон. Осталось здоровья: {health}");

        // Звук урона
        GameObject spawnedSound = Instantiate(harvestSound);

        // Спавн доски
        await SpawnWood();

        // Тряска дерева
        await ShakeTree();
        Destroy(spawnedSound);

        // Если здоровье упало до нуля
        if (health <= 0)
        {
            GameObject spawnedEndSound = Instantiate(harvestEndSound);
            Destroy(spawnedEndSound, 3f);

            Deplete();
        }
    }

    private async UniTask SpawnWood()
    {
        int woodToSpawn = 1; // Количество досок, которые спавнятся при уроне
        for (int i = 0; i < woodToSpawn; i++)
        {
            Vector3 spawnPosition = Vector3.zero;
            bool validPositionFound = false;

            // Пытаемся найти валидную позицию
            for (int attempt = 0; attempt < 10; attempt++) // Ограничиваем количество попыток
            {
                // Рассчитываем случайное направление
                Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;

                // Рассчитываем потенциальную позицию
                Vector3 potentialPosition = transform.position + (Vector3)randomDirection * UnityEngine.Random.Range(0.5f, spawnRadius);

                // Проверяем достижимость точки через A*
                if (AstarPath.active != null)
                {
                    var graph = AstarPath.active.data.gridGraph;
                    var node = graph.GetNearest(potentialPosition).node;

                    if (node != null && node.Walkable)
                    {
                        spawnPosition = (Vector3)node.position;
                        validPositionFound = true;
                        break;
                    }
                }
            }

            // Если не нашли валидную позицию, используем позицию дерева
            if (!validPositionFound)
            {
                spawnPosition = transform.position;
            }

            // Создаем префаб доски
            GameObject wood = Instantiate(resourcePrefab, transform.position, Quaternion.identity);
            
            // Используем DOTween для движения доски
            wood.transform.DOMove(spawnPosition, 0.5f).SetEase(Ease.OutQuad);

            // Небольшая задержка между спавном досок
            await UniTask.Delay(100);
        }
    }
    
    
    protected override void Deplete()
    {
        Debug.Log($"Дерево {name} уничтожено.");
        gameObject.layer = 0;

        AstarPath.active.Scan();
        Destroy(gameObject, 0.1f);
    }
    
    private async UniTask ShakeTree()
    {
        // Тряска дерева с использованием DOTween
        Vector3 originalPosition = transform.position;

        // Выполним анимацию с тряской, используя DOTween
        transform.DOShakePosition(shakeDuration, shakeStrength, 20, 90, false, true);

        // Дожидаемся завершения тряски
        await UniTask.Delay((int)(shakeDuration * 1000));

        // Вернем дерево в исходное положение
        transform.position = originalPosition;
    }
    
}