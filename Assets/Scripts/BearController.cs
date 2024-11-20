using UnityEngine;

public class BearController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform target;
    
    private BearState currentState;
    private CommandQueue commandQueue;
    private Command currentCommand = null;

    private void Awake()
    {
        commandQueue = new CommandQueue();
        SetState(new IdleState(this));
    }
    
    public void SetState(BearState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
        UIManager.Instance.currentStateTMPro.text = currentState.GetType().ToString();
    }
    
    public BearState GetState()
    {
        return currentState;
    }

    public void Select()
    {
        Debug.Log($"{gameObject.name} selected.");
        GetComponent<SpriteRenderer>().color = Color.green;
        UIManager.Instance.currentStateTMPro.text = currentState.GetType().ToString();
        // Добавьте визуальные эффекты или выделение
    }

    public void Deselect()
    {
        Debug.Log($"{gameObject.name} deselected.");
        GetComponent<SpriteRenderer>().color = Color.white;
        // Уберите визуальные эффекты
    }

    private void Update()
    {
        currentState?.Update();
    }

    public void AddCommand(Command command)
    {
        // Если команда - MoveCommand, то отменяем все предыдущие команды
        if (command is MoveCommand)
        {
            // Очищаем очередь команд
            commandQueue.Clear();
            SetState(new IdleState(this));
            Debug.Log("Все предыдущие команды отменены, добавляется новая MoveCommand.");
        }
        
        commandQueue.EnqueueCommand(command);
    }

    public void ClearCommands()
    {
        commandQueue.Clear();
    }
}