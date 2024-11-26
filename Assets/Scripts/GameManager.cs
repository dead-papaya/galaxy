using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool isBuilding()
    {
        return UIManager.Instance.buildingMenu.gameObject.activeSelf;
    }
    
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("More than one UIManager!");
            Destroy(gameObject);
        }
    }
    
    
}
