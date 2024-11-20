using System.Threading.Tasks;
using UnityEngine;

public class HarvestState : BearState
{
    private TreeResource targetTree;
    private float harvestDuration = 1f; // Длительность рубки дерева в секундах
    private bool isHarvesting = false;

    public HarvestState(BearController bear, TreeResource targetTree) : base(bear)
    {
        this.targetTree = targetTree;
    }

    public override void Enter()
    {
        if (targetTree == null)
        {
            Debug.LogError("Нет цели для рубки дерева!");
            bear.SetState(new IdleState(bear));
            return;
        }

        Debug.Log($"{bear.name} начал рубить дерево {targetTree.name}.");
        StartHarvesting();
    }

    private async void StartHarvesting()
    {
        isHarvesting = true;

        // Анимация начала рубки (опционально)
        //bear.Animator.SetTrigger("Chop");

        // Ожидание завершения рубки
        await Task.Delay((int)(harvestDuration * 1000));
        if(targetTree == null) Exit();
        if (isHarvesting)
        {
            // Уменьшаем здоровье дерева
            targetTree.TakeDamage();

            // Проверяем, можно ли продолжить рубку
            if (!targetTree.IsDepleted())
            {
                Debug.Log($"{bear.name} продолжает рубить дерево.");
                StartHarvesting();
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
        Debug.Log($"{bear.name} прекратил рубку дерева.");
        isHarvesting = false;
    }
}