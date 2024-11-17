using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TestAPICalls : MonoBehaviour
{
    private string url_register = "https://2025.nti-gamedev.ru/api/games/7317031d-266a-41b0-a311-c16c6e3b974a/players/";
    private string filePath = "Assets/save.json";

    [Header("REGISTER")] [SerializeField] private TMP_InputField register_nickname_inputField;
    [SerializeField] private TMP_InputField register_password_inputField;
    [SerializeField] private Button register_registerButton;
    [SerializeField] private TextMeshProUGUI register_errorLog_inputField;

    [Header("LOGIN")] [SerializeField] private TMP_InputField login_nickname_inputField;
    [SerializeField] private TMP_InputField login_password_inputField;
    [SerializeField] private Button login_registerButton;
    [SerializeField] private TextMeshProUGUI login_errorLog_inputField;



    private Player _player = new Player();

    private void Start()
    {
        register_registerButton.onClick.AddListener(delegate { StartCoroutine(Register()); });
    }

    private void SaveToJson()
    {
        string json = JsonUtility.ToJson(_player, true); // true для форматирования
        File.WriteAllText(filePath, json);
        Debug.Log($"Данные записаны в {filePath}");
    }

    private void LoadFromJson()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            _player = JsonUtility.FromJson<Player>(json);
            Debug.Log($"Данные загружены из {filePath}");
        }
        else
        {
            Debug.LogWarning("Файл не найден.");
        }
    }

    private IEnumerator Register()
    {
        string nickname = register_nickname_inputField.text;
        string password = register_password_inputField.text;

        _player.name = nickname;
        _player.password = password;
        Resource t = new Resource();
        t.name = "JAR";
        t.count = 19;
        _player.resources = new List<Resource>();
        _player.resources.Add(t);

        SaveToJson();

        string jsonData = File.ReadAllText(filePath);

        UnityWebRequest request = new UnityWebRequest(url_register, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
        request.SetRequestHeader("Authorization", "7317031d-266a-41b0-a311-c16c6e3b974a");
        // Устанавливаем тело запроса
        request.uploadHandler = new UploadHandlerRaw(jsonToSend);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // Отправляем запрос
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Запрос успешно отправлен: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Ошибка: " + request.error);
            if (request.error.Contains("400"))
            {
                register_errorLog_inputField.text = "Пользователь с таким именем уже существует.";
            }
        }
    }
}

public class Player
{
    public string name;
    public string password;
    public List<Resource> resources;
}

public class Resource
{
    public string name;
    public int count;
}