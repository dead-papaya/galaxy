using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class ResourceToUI : MonoBehaviour
{
    public string name;
    [SerializeField] private float moveDuration = 0.8f; // Длительность анимации
    [SerializeField] private Ease moveEase = Ease.InOutQuad; // Тип easing для анимации
    private Transform targetUI; // Целевая позиция в UI
    private Canvas canvas;

    private async void Awake()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(1f));
        MoveToUIAndDestroy();
    }

    public async void MoveToUIAndDestroy()
    {
        // Получаем цель в UI (иконку ресурса)
        RectTransform targetUI = UIManager.Instance.GetResourceIconTarget(name);

        Vector3 worldPos = Camera.main.ScreenToWorldPoint(targetUI.GetComponent<ResourceIcon>().iconImage.transform.position);
        
        // Анимация перемещения ресурса в UI
        await transform.DOMove(worldPos, moveDuration).SetEase(Ease.OutQuad).AsyncWaitForCompletion();

        // Добавляем ресурс в интерфейс после анимации
        UIManager.Instance.AddResource(name);

        // Удаляем ресурс из мира
        Destroy(gameObject);
    }

}