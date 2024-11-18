using UnityEngine;

public class MoveCommand : Command
{
    private Vector3 target;

    public MoveCommand(Vector3 target)
    {
        this.target = target;
    }

    public override void Execute(BearController bear)
    {
        bear.SetState(new MoveState(bear, this.target));
    }
}