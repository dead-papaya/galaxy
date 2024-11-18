using UnityEngine;

public class BearSelector : MonoBehaviour
{
    void OnMouseDown()
    {
        print("OnMouseDown");
        BearController bear = GetComponent<BearController>();
        if (bear != null)
        {
            BearManager.Instance.SelectBear(bear);
            Debug.Log($"Selected: {bear.name}");
        }
        else
        {
            Debug.LogWarning("BearController not found on the object!");
        }
    }
}