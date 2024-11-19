using UnityEngine;

public abstract class Command
{
    public string commandName { get; protected set; }
    public abstract void Execute(BearController bear);
}