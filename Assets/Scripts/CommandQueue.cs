using System.Collections.Generic;
using UnityEngine;

public class CommandQueue
{
    private Queue<Command> commandQueue = new Queue<Command>();
    private bool isExecuting = false;

    public void EnqueueCommand(Command command)
    {
        commandQueue.Enqueue(command);
        TryExecuteNext();
    }

    private async void TryExecuteNext()
    {
        Debug.Log("Try execute next command");
        if (isExecuting || commandQueue.Count == 0)
        {
            Debug.Log($"isExecuting {isExecuting} , commandQueue.Count {commandQueue.Count} => RETURN");
            return;
        }

        isExecuting = true;
        Command command = commandQueue.Dequeue();
        await command.ExecuteAsync();
        isExecuting = false;

        // Рекурсивно вызываем, если в очереди остались команды
        TryExecuteNext();
    }

    public void Clear()
    {
        Debug.Log("CLEAR COMMAND LIST");
        commandQueue.Clear();
        isExecuting = false;
        BearManager.Instance.GetSelectedBear().SetState(new IdleState(BearManager.Instance.GetSelectedBear()));
    }
}