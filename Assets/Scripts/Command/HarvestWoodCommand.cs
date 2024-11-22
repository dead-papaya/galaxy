using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
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
        var moveCommand = new MoveCommand(bear, targetTree.transform.position, 1f);
        await moveCommand.ExecuteAsync();
        
        bear.currentCommand = this; // ставим команду медведю после выполнение MoveCommand
        await UniTask.Delay(500);

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
            if (bear.currentCommand != this)
            {
                Debug.Log("HarvestWoodCommand отменена.");
                return;
            }

            await Task.Yield();
        }

        bear.currentCommand = null;
        Debug.Log($"{bear.name} завершил добычу дерева {targetTree.name}. bear.currentCommand != this");
    }

    public override void Cancel()
    {
        if(bear.currentCommand == this) bear.currentCommand = null;
        bear.SetState(new IdleState(bear)); // При отмене возвращаем медведя в Idle состояние
        Debug.Log("HarvestWoodCommand отменена.");
    }
}