using System.Threading.Tasks;
using UnityEngine;

public class HarvestState : BearState
{
    private ResourceObject resourceObject;
    private float harvestDuration = 0.5f; // Длительность рубки дерева в секундах
    private bool isHarvesting = false;

    public HarvestState(BearController bear, ResourceObject resource) : base(bear)
    {
        this.resourceObject = resource;
    }

    public override void Enter()
    {
        if (resourceObject == null)
        {
            Debug.LogError("Нет цели для рубки дерева!");
            bear.SetState(new IdleState(bear));
            return;
        }
        
        bear.bearAnimations.SetFacingDirection(resourceObject.transform.position);
        Debug.Log($"{bear.name} начал рубить дерево {resourceObject.name}.");
        StartHarvesting();
    }

    private async void StartHarvesting()
    {
        isHarvesting = true;

        // Анимация начала рубки (опционально)
        //bear.Animator.SetTrigger("Chop");

        // Ожидание завершения рубки
        bear.bearAnimations.StartHarvesting();
        await Task.Delay((int)(harvestDuration * 1000));
        if(resourceObject == null) Exit();
        if (isHarvesting)
        {
            // Уменьшаем здоровье дерева
            resourceObject.TakeDamage();

            // Проверяем, можно ли продолжить рубку
            if (!resourceObject.IsDepleted())
            {
                Debug.Log($"{bear.name} продолжает рубить дерево.");
                await Task.Delay((int)(harvestDuration * 1000));
                if(isHarvesting) StartHarvesting();
            }
            else
            {
                Debug.Log($"{bear.name} завершил рубку дерева.");
                isHarvesting = false;
                if(!(bear.GetState() is MoveState)) bear.SetState(new IdleState(bear));
            }
        }
        else
        {
            Debug.Log($"{bear.name} завершил рубку дерева.");
            isHarvesting = false;
            if(!(bear.GetState() is MoveState)) bear.SetState(new IdleState(bear));
        }
    }

    public override void Update()
    {
        if (!isHarvesting)
        {
            if(!(bear.GetState() is MoveState))bear.SetState(new IdleState(bear));
        }
    }

    public override void Exit()
    {
        isHarvesting = false;
        bear.bearAnimations.StopHarvesting();
        Debug.Log($"{bear.name} прекратил рубку дерева. Exits");
    }
}