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
        targetPosition = GetNearestReachablePoint(bear.transform.position, target);
        targetPosition.z = 0;
        endDistance = 1f;  // Стандартное значение, можно изменить
        commandName = "Идти сюда";
    }

    public MoveCommand(BearController bear, Vector3 target, float endDistance)
    {
        this.bear = bear;
        targetPosition = GetNearestReachablePoint(bear.transform.position, target);
        targetPosition.z = 0;
        this.endDistance = endDistance;
        commandName = "Идти сюда";
    }

    public override async Task ExecuteAsync()
    {
        // Устанавливаем состояние движения
        Debug.Log($"{bear.name} начал движение.");
        bear.SetState(new MoveState(bear, targetPosition, endDistance));

        // Ожидаем, пока медведь достигнет цели или команда не будет отменена
        while (Vector3.Distance(bear.transform.position, targetPosition) > endDistance)
        {
            await Task.Yield(); // Ожидаем следующего кадра
        }

        Debug.Log($"{bear.name} завершил движение.");
    }
    
    public override void Cancel()
    {
        // Если команда отменяется, устанавливаем медведя в Idle состояние
        bear.SetState(new IdleState(bear));
        Debug.Log("Command MOVE canceled");
    }
    
    private Vector3 GetNearestReachablePoint(Vector3 start, Vector3 target)
    {
        var graph = AstarPath.active.data.gridGraph;

        // Преобразуем стартовую точку в узел
        var startNode = graph.GetNearest(start).node;

        // Получаем список всех достижимых узлов из стартовой точки
        var reachableNodes = PathUtilities.GetReachableNodes(startNode);

        // Ищем ближайшую достижимую точку к целевой
        GraphNode nearestNode = null;
        float minDistance = float.MaxValue;

        foreach (var node in reachableNodes)
        {
            var worldPosition = (Vector3)node.position;
            float distance = Vector3.Distance(worldPosition, target);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearestNode = node;
            }
        }

        Debug.Log(nearestNode != null ? (Vector3)nearestNode.position : start);
        return nearestNode != null ? (Vector3)nearestNode.position : start;
    }
}