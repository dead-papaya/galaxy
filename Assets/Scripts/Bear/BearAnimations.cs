using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BearAnimations : MonoBehaviour
{
    private Animator _animator;
    private BearController _bearController;
    private SpriteRenderer _spriteRenderer;
    private Vector3 previousPosition = new Vector3();
    private CancellationTokenSource cancellationTokenSource;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _bearController = GetComponent<BearController>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    // Устанавливает направление медведя (лицом к игроку или спиной)
    public void SetFacingDirection()
    {
        print("SET FACING DIRECTION");
        // Для движения
        if (previousPosition.y < transform.position.y - 0.1f)
        {
            // Медведь идет спиной
            _animator.SetBool("isFacingPlayer", false);
        }
        else if(previousPosition.y > transform.position.y - 0.1f)
        {
            // Медведь идет лицом
            _animator.SetBool("isFacingPlayer", true);
        }

        if (previousPosition.x < transform.position.x)
        {
            _spriteRenderer.flipX = true;
        }
        else if(previousPosition.x > transform.position.x)
        {
            _spriteRenderer.flipX = false;
        }

        previousPosition = transform.position;
    }
    
    // Запуск обновления направления с использованием UniTask
    public void StartFacingDirectionUpdates()
    {
        cancellationTokenSource?.Cancel(); // Отменяем предыдущую задачу, если она существует
        cancellationTokenSource = new CancellationTokenSource(); // Создаём новый источник отмены

        UpdateFacingDirection(cancellationTokenSource.Token).Forget(); // Запускаем UniTask с токеном отмены
    }

    // Асинхронный метод для обновления направления
    private async UniTaskVoid UpdateFacingDirection(CancellationToken token)
    {
        while (!token.IsCancellationRequested) // Проверяем, не была ли отменена задача
        {
            SetFacingDirection(); // Обновляем направление
            await UniTask.Delay(200, cancellationToken: token);  // Ожидаем 200 миллисекунд (0.2 секунды)
        }
    }

    // Останавливаем обновление направления
    public void StopFacingDirectionUpdates()
    {
        cancellationTokenSource?.Cancel(); // Отменяем задачу
    }


    public void UpdateAnimatorState()
    {
        // Пример: обновляем состояние анимации в зависимости от состояния медведя
        if (_bearController.GetState() is MoveState)
        {
            _animator.SetBool("isMoving", true);
            _animator.SetBool("isHarvesting", false);
        }
        else if (_bearController.GetState() is HarvestState)
        {
            _animator.SetBool("isMoving", false);
            _animator.SetBool("isHarvesting", true);
        }
        else
        {
            _animator.SetBool("isMoving", false);
            _animator.SetBool("isHarvesting", false);
        }
    }

    // Вызовите этот метод, когда медведь начнёт двигаться
    public void StartMoving()
    {
        StartFacingDirectionUpdates();
        _animator.SetBool("isMoving", true);
    }

    // Когда медведь остановится или вернется в Idle
    public void StopMoving()
    {
        StopFacingDirectionUpdates();
        _animator.SetBool("isMoving", false);
    }

    // Для начала анимации рубки дерева
    public void StartHarvesting()
    {
        _animator.SetBool("isHarvesting", true);
    }

    // Для завершения анимации рубки
    public void StopHarvesting()
    {
        _animator.SetBool("isHarvesting", false);
    }
}