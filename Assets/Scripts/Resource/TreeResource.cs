using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class TreeResource : MonoBehaviour
{
    public int health = 5; // Количество ударов, чтобы дерево исчезло
    public float shakeDuration = 0.5f;  // Длительность тряски
    public float shakeStrength = 0.2f;
    [SerializeField] private GameObject harvestSound;
    [SerializeField] private GameObject harvestEndSound;
    [SerializeField] private LayerMask playerLayerMask;

    private void Awake()
    {
        GetComponent<CommandList>().Commands = new List<Command>(){(new HarvestWoodCommand(null, this))};
        if (Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y), new Vector2(2f, 2f), 0f, playerLayerMask))
        {
            print("DESTROY TREE AT POS: " + transform.position);
            Destroy(gameObject);
        }
    }

    public async void TakeDamage()
    {
        health--;
        Debug.Log($"Дерево {name} получило урон. Осталось здоровья: {health}");
        //UIManager.Instance.treeResourceCountText.text = (Convert.ToInt32(UIManager.Instance.treeResourceCountText.text) + 1).ToString();
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

    public bool IsDepleted()
    {
        return health <= 0;
    }

    private void Deplete()
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