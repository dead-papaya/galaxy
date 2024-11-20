using System;
using UnityEngine;

public class BearController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform target;
    
    private BearState currentState;
    public CommandQueue commandQueue = new CommandQueue();
    public Command currentCommand = null;

    private void Awake()
    {
        commandQueue = new CommandQueue();
    }

    private void Start()
    {
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
        UIManager.Instance.currentCommandListLengthTMPro.text = "Current command queue length: " + commandQueue.commandQueue.Count.ToString();
    }

    public void AddCommand(Command command)
    {
        commandQueue.AddCommand(command);
    }
    
}