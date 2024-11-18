using UnityEngine;

public class BearController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public Transform target;
    
    private BearState currentState;

    public void SetState(BearState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Select()
    {
        Debug.Log($"{gameObject.name} selected.");
        GetComponent<SpriteRenderer>().color = Color.green;
        // Добавьте визуальные эффекты или выделение
    }

    public void Deselect()
    {
        Debug.Log($"{gameObject.name} deselected.");
        GetComponent<SpriteRenderer>().color = Color.white;
        // Уберите визуальные эффекты
    }

    void Update()
    {
        currentState?.Update();
    }
}