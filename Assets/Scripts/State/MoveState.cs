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
        bear.target = targetPosition;
        Debug.Log($"{bear.name} starts moving.");
    }

    public override void Update()
    {
        if (Vector3.Distance(bear.transform.position, targetPosition) < 0.1f)
        {
            bear.SetState(new IdleState(bear)); // Переход в Idle после достижения точки
        }
    }

    public override void Exit() => Debug.Log($"{bear.name} stops moving.");
}