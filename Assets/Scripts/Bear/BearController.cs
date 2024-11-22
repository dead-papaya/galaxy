using System;
using TMPro;
using UnityEngine;

public class BearController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform target;

    public BearState currentState { get; private set; }
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
    }
    
    public BearState GetState()
    {
        return currentState;
    }

    public void Select()
    {
        Debug.Log($"{gameObject.name} selected.");
        GetComponent<SpriteRenderer>().color = new Color(160f/255f, 1f, 160f/255f);
        UIManager.Instance.contextMenu.SetActive(false);
        //UIManager.Instance.currentStateTMPro.text = currentState.GetType().ToString();
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
        if (BearManager.Instance.GetSelectedBear() == this)
        {
            if (currentCommand != null) UIManager.Instance.commandText.text = currentCommand.ToString();
            else UIManager.Instance.commandText.text = "Null";

            if (currentState != null) UIManager.Instance.stateText.text = currentState.ToString();
            else UIManager.Instance.stateText.text = "Null";
        }
    }
    
}