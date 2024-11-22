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

    public override async void TakeDamage()
    {
        health--;
        Debug.Log($"Дерево {name} получило урон. Осталось здоровья: {health}");
        //Spawn resource
        //UIManager.Instance.woodText.text = (Convert.ToInt32(UIManager.Instance.woodText.text) + 1).ToString();
        //Добавить дерево игроку
        GameObject spawnedSound = Instantiate(harvestSound);
        if (health <= 0)
        {
            GameObject spawnedEndSound = Instantiate(harvestEndSound);
            Destroy(spawnedEndSound, 3f);
        }
        
        await ShakeTree();
        Destroy(spawnedSound);

        if (health <= 0)
        {
            Deplete();
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