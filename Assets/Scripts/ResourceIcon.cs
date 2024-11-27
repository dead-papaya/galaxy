using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceIcon : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countText; // Текст для количества
    public Image iconImage; // Изображение ресурса

    private int count;

    private Tween scaleTween;

    public GameObject currentResourceGO;

    public float fireHealth = -1f;

    /// <summary>
    /// Инициализация иконки ресурса.
    /// </summary>
    public void Initialize()
    {
        count = 0;
        countText.text = count.ToString();

        // Установим иконку ресурса по имени
        Sprite resourceSprite = Resources.Load<Sprite>($"Icons/{name}");
        if (resourceSprite != null)
        {
            iconImage.sprite = resourceSprite;
        }
        else
        {
            Debug.LogWarning($"Иконка для ресурса '{name}' не найдена.");
        }
        
        // Установим префаб ресурса по имени
        GameObject resourcePrefab = Resources.Load<GameObject>($"Resources/{name}");
        if (resourcePrefab != null)
        {
            currentResourceGO = resourcePrefab;
        }
        else
        {
            Debug.LogWarning($"Префаб для ресурса '{name}' не найден.");
        }

        switch (name)
        {
            case "Wood":
                fireHealth = 20f;
                break;
            case "Coal":
                fireHealth = 90f;
                break;
        }
    }

    public int GetCount()
    {
        return count;
    }

    public void IncreaseAmount(int amount)
    {
        GameObject spawnedSound = Instantiate(UIManager.Instance.takeResourceSoundPrefab);
        Destroy(spawnedSound, 0.5f);
        count += amount; // Увеличиваем количество
        UpdateAmountText(); // Обновляем текстовое поле
        ScaleTask();
    }

    private async UniTask ScaleTask()
    {
        if (scaleTween == null)
        {
            scaleTween = iconImage.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.1f);
            await scaleTween;
            scaleTween = iconImage.transform.DOScale(new Vector3(1, 1, 1), 0.1f);
            await scaleTween;
            scaleTween = null;
        }
    }
    
    // Обновление текстового поля с количеством
    private void UpdateAmountText()
    {
        countText.text = count.ToString();
    }
}