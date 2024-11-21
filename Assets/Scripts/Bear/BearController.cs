using System;
using UnityEngine;

public class BearController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform target;
    
    private BearState currentState;
    public Command currentCommand = null;

    public BearAnimations bearAnimations;


    private void Awake()
    {
        bearAnimations = GetComponent<BearAnimations>();
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
    }
    
}