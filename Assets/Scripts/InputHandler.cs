using UnityEngine;

public class InputHandler : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) // ПКМ для перемещения
        {
            Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            target.z = 0;

            BearController selectedBear = BearManager.Instance.GetSelectedBear();
            if (selectedBear != null)
            {
                Command moveCommand = new MoveCommand(target);
                moveCommand.Execute(selectedBear);
            }
        }
    }
}