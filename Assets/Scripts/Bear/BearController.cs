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
    
    private SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    public Material outlineMaterial; // Установите материал с шейдером OutlineShader


    private void Awake()
    {
        bearAnimations = GetComponent<BearAnimations>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;

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
        Furnace[] furs = GameObject.FindObjectsByType<Furnace>(FindObjectsSortMode.None);
        foreach (var fur in furs)
        {
            fur.Deselect();
        }  
        UIManager.Instance.contextMenu.SetActive(false);
        if (outlineMaterial != null)
        {
            spriteRenderer.material = outlineMaterial;
        }
    }

    public void Deselect()
    {
        Debug.Log($"{gameObject.name} deselected.");
        spriteRenderer.material = originalMaterial;
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