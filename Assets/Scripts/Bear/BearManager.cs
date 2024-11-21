using System;
using UnityEngine;

public class BearManager : MonoBehaviour
{
    public static BearManager Instance { get; private set; }

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

    public BearController GetSelectedBear()
    {
        return selectedBear;
    }
}