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
        await new MoveCommand(bear, targetTree.transform.position, 2f).ExecuteAsync();
        
        if (bear == null || targetTree == null)
        {
            Debug.LogError($"bear == {bear}, targetTree = {targetTree}");
        }
        bear.SetState(new HarvestState(bear, targetTree));

        // Ожидаем завершения добычи дерева
        while (!targetTree.IsDepleted())
        {
            await Task.Yield();
        }

        Debug.Log($"{bear.name} завершил добычу дерева {targetTree.name}.");
    }
}