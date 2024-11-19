using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticSortingOrder : SortingOrder
{
    private void Start()
    {
        print("Static Sort: " + gameObject.name);
        Sort();
    }
}
