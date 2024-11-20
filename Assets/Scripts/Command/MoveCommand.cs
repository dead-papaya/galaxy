using Pathfinding;
using System.Threading.Tasks;
using UnityEngine;

public class MoveCommand : Command
{
    public BearController bear;
    private Vector3 targetPosition;
    private float endDistance;

    public MoveCommand(BearController bear, Vector3 target)
    {
        this.bear = bear;
        targetPosition = target;
        targetPosition.z = 0;
        endDistance = 1f;
        commandName = "Идти сюда";
    }
    
    public MoveCommand(BearController bear, Vector3 target, float endDistance)
    {
        this.bear = bear;
        targetPosition = target;
        targetPosition.z = 0;
        this.endDistance = endDistance;
        commandName = "Идти сюда";
    }

    public override async Task ExecuteAsync()
    {
        bear.SetState(new MoveState(bear, targetPosition, endDistance));

        // Ожидаем, пока медведь достигнет цели
        while (Vector3.Distance(bear.transform.position, targetPosition) > endDistance)
        {
            await Task.Yield();
        }

        Debug.Log($"{bear.name} завершил движение.");
    }
    
}