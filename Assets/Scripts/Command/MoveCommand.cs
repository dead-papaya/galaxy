using Pathfinding;
using System.Threading.Tasks;
using UnityEngine;

public class MoveCommand : Command
{
    public BearController bear;
    private Vector3 targetPosition;
    private float endDistance;
    private CommandQueue commandQueue;  // Добавим ссылку на очередь команд

    public MoveCommand(BearController bear, Vector3 target)
    {
        this.bear = bear;
        targetPosition = target;
        targetPosition.z = 0;
        endDistance = 1f;  // Стандартное значение, можно изменить
        this.commandQueue = bear.commandQueue; // Передаем очередь команд
        commandName = "Идти сюда";
    }

    public MoveCommand(BearController bear, Vector3 target, float endDistance)
    {
        this.bear = bear;
        targetPosition = target;
        targetPosition.z = 0;
        this.endDistance = endDistance;
        this.commandQueue = bear.commandQueue; // Передаем очередь команд
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
            // Проверяем, является ли эта команда текущей выполняемой командой
            if (commandQueue.currentCommand != this)
            {
                return; // Если текущая команда не эта, выходим (команда отменена)
            }

            await Task.Yield(); // Ожидаем следующего кадра
        }

        Debug.Log($"{bear.name} завершил движение.");
    }
    
    public override void Cancel()
    {
        // Если команда отменяется, устанавливаем медведя в Idle состояние
        bear.SetState(new IdleState(bear));
        Debug.Log("Command MOVE canceled"); // При отмене возвращаем медведя в Idle состояние
    }
}