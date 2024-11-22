using Pathfinding;
using UnityEngine;

public class MoveState : BearState
{
    private Vector3 targetPosition;
    private float endDistance;
    public GridGraph graph;

    public MoveState(BearController bear, Vector3 target, float endDistance) : base(bear)
    {
        targetPosition = target;
        this.endDistance = endDistance;
    }

    public override void Enter()
    {
        bear.target.position = targetPosition;
        bear.GetComponent<AIDestinationSetter>().target = bear.target;
        bear.bearAnimations.StartMoving();
        Debug.Log($"{bear.name} starts moving.");
        graph = AstarPath.active.data.gridGraph;
        
    }

    public override void Update()
    {
        bear.transform.position = new Vector3(bear.transform.position.x, bear.transform.position.y, 0);
        if (Vector3.Distance(bear.transform.position, targetPosition) < endDistance)
        {
            var node = graph.GetNearest(bear.transform.position).node;

            // Проверяем, находится ли агент на проходимом узле
            if (!node.Walkable)
            {
                // Перемещаем агента на ближайший проходимый узел
                node = graph.GetNearest(bear.transform.position).node;
                bear.transform.position = (Vector3)node.position;
                Debug.LogWarning("Agent was placed on a non-walkable node. Repositioning...");
            }
            
            Exit();
            bear.SetState(new IdleState(bear)); // Переход в Idle после достижения точки
        }
    }

    public override void Exit()
    { 
        Debug.LogWarning("Agent was placed on a non-walkable node. Repositioning...");
        bear.GetComponent<AIDestinationSetter>().target = null;
        bear.bearAnimations.StopMoving();
        var node = graph.GetNearest(bear.transform.position).node;
        bear.transform.position = (Vector3)node.position;
        Debug.Log($"{bear.name} stops moving.");
    }
}