using System.Threading.Tasks;
using UnityEngine;

public class HarvestWoodCommand : Command
{
    private TreeResource targetTree;

    public HarvestWoodCommand(BearController bear, TreeResource tree)
    {
        this.bear = bear;
        this.targetTree = tree;
        commandName = "Срубить дерево";
    }

    public override async Task ExecuteAsync()
    {
        // Создаём команду MoveCommand для подхода к дереву
        var moveCommand = new MoveCommand(bear, targetTree.transform.position, 2f);
        await moveCommand.ExecuteAsync();

        // Проверяем, был ли медведь отменён или дерево недоступно
        if (bear == null || targetTree == null || targetTree.IsDepleted())
        {
            Debug.LogWarning("Команда Harvest отменена или цель недоступна.");
            return;
        }

        // Переход в состояние рубки дерева
        bear.SetState(new HarvestState(bear, targetTree));
        Debug.Log($"{bear.name} начал рубить дерево {targetTree.name}.");

        // Ожидание завершения рубки дерева
        while (!targetTree.IsDepleted())
        {
            // Проверяем, если команда была отменена
            if (bear.currentCommand != this) // Исправлено: используем ссылку на queue из BearController
            {
                Debug.Log("HarvestWoodCommand отменена.");
                return;
            }

            await Task.Yield();
        }

        Debug.Log($"{bear.name} завершил добычу дерева {targetTree.name}.");
    }

    public override void Cancel()
    {
        bear.SetState(new IdleState(bear)); // При отмене возвращаем медведя в Idle состояние
        Debug.Log("HarvestWoodCommand отменена.");
    }
}