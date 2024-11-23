using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class StoneResource : ResourceObject
{
    public GameObject pickaxeSound;
    private void Awake()
    {
        GetComponent<CommandList>().Commands = new List<Command>(){(new HarvestResourceCommand(null, this))};
        if (Physics2D.OverlapBox(new Vector2(transform.position.x, transform.position.y), new Vector2(1f, 1f), 0f, playerLayerMask))
        {
            print("DESTROY STONE AT POS: " + transform.position);
            Destroy(gameObject);
        }
        if (Physics2D.OverlapBox(new Vector2(harvestTransform.position.x, harvestTransform.position.y), new Vector2(0.1f, 0.1f), 0f, gameObject.layer))
        {
            print("DESTROY STONE AT POS: " + transform.position);
            Destroy(gameObject);
        }
    }

    public override async void TakeDamage()
    {
        health--;
        Debug.Log($"Камень {name} получил урон. Осталось здоровья: {health}");
        //Spawn resource
        //UIManager.Instance.woodText.text = (Convert.ToInt32(UIManager.Instance.woodText.text) + 1).ToString();
        //Добавить дерево игроку
        GameObject spanedPickaxeSound = Instantiate(pickaxeSound);
        Destroy(spanedPickaxeSound, 0.5f);
        
        GameObject spawnedSound = Instantiate(harvestSound);
        Destroy(spawnedSound, 1f);
        if (health <= 0)
        {
            GameObject spawnedEndSound = Instantiate(harvestEndSound);
            Destroy(spawnedEndSound, 3f);
        }
        
        await Shake();

        if (health <= 0)
        {
            Deplete();
        }
    }
    
    protected override void Deplete()
    {
        Debug.Log($"Камень {name} уничтожен.");
        gameObject.layer = 0;

        AstarPath.active.Scan();
        Destroy(gameObject, 0.1f);
    }
    
    private async UniTask Shake()
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
