using System;
using UnityEngine;

public class BearManager : MonoBehaviour
{
    public static BearManager Instance { get; private set; }
    public LayerMask playerLayerMask;
    public BearController selectedBear; // Текущий выбранный медведь

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public void SelectBear(BearController bear)
    {
        if (selectedBear == bear)
        {
            // Если медведь уже выбран, ничего не делать
            print("SELECTED BEAR = BEAR");
            return;
        }

        if (selectedBear != null)
        {
            selectedBear.Deselect(); // Снимаем выделение
        }

        selectedBear = bear;
        if (selectedBear != null)
        {
            selectedBear.Select(); // Устанавливаем выделение
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 20f, playerLayerMask);
            
            BearController bear;
            foreach (var hit in hits)
            {
                if (hit.transform.gameObject.TryGetComponent<BearController>(out bear))
                {
                    BearManager.Instance.SelectBear(bear);
                    break;
                }
            }

        }
    }

    public BearController GetSelectedBear()
    {
        return selectedBear;
    }
}