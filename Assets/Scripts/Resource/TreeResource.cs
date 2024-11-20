using System;
using System.Collections.Generic;
using UnityEngine;

public class TreeResource : MonoBehaviour
{
    public int health = 5; // Количество ударов, чтобы дерево исчезло

    private void Start()
    {
        GetComponent<CommandList>().Commands = new List<Command>(){(new HarvestWoodCommand(null, this))};
    }

    public void TakeDamage()
    {
        health--;
        Debug.Log($"Дерево {name} получило урон. Осталось здоровья: {health}");
        UIManager.Instance.treeResourceCountText.text = (Convert.ToInt32(UIManager.Instance.treeResourceCountText.text) + 1).ToString();

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
        Destroy(gameObject, 0.1f);
    }
}