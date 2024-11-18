using Pathfinding;
using UnityEngine;

public class MoveState : BearState
{
    private Vector3 targetPosition;

    public MoveState(BearController bear, Vector3 target) : base(bear)
    {
        targetPosition = target;
    }

    public override void Enter()
    {
        bear.target.position = targetPosition;
        bear.GetComponent<AIDestinationSetter>().target = bear.target;
        Debug.Log($"{bear.name} starts moving.");
    }

    public override void Update()
    {
        bear.transform.position = new Vector3(bear.transform.position.x, bear.transform.position.y, 0);
        if (Vector3.Distance(bear.transform.position, targetPosition) < 0.1f)
        {
            bear.SetState(new IdleState(bear)); // Переход в Idle после достижения точки
        }
    }

    public override void Exit()
    { 
        bear.GetComponent<AIDestinationSetter>().target = null;
        Debug.Log($"{bear.name} stops moving.");
    }
}