using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;
using UnityEngine.Networking;
using System.Reflection;
using UnityEngine.EventSystems;

public class ServerSaveFileHandler : SaveFileHandler
{
    readonly string baseUrl;
    readonly MonoBehaviour coroutinePlayer;
    public ServerSaveFileHandler(string baseUrl, MonoBehaviour coroutinePlayer)
    {
        this.baseUrl = baseUrl;
        this.coroutinePlayer = coroutinePlayer;
    }
    public override void Load(string fileName, Action<string> onLoad = null)
    {
        coroutinePlayer.StartCoroutine(LoadDataFromServer(fileName, (data) => onLoad?.Invoke(data)));
    }
    public override void Save(string data, string fileName, Action onSave = null)
    {
        coroutinePlayer.StartCoroutine(SaveDataToServer(fileName, data, () => onSave?.Invoke()));
    }
    public override void Delete(string fileName, Action onDelete = null)
    {
        throw new NotImplementedException();
    }

    private IEnumerator LoadDataFromServer(string fileName, Action<string> onLoad)
    {
        var requestData = new { uid = SystemInfo.deviceUniqueIdentifier + "/" + fileName };

        string jsonData = JsonUtility.ToJson(requestData);

        UnityWebRequest request = new UnityWebRequest(baseUrl + "/LoadMap", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string result = request.downloadHandler.text;
            if (result[0] == '[') result = null;
            Debug.Log(result);
            onLoad?.Invoke(result);
        }
        else
        {
            Debug.LogError("Error fetching movies" + request.downloadHandler.text);
        }
    }
    public IEnumerator SaveDataToServer(string fileName, string content, Action onSave)
    {
        var requestData = new { uid = SystemInfo.deviceUniqueIdentifier + "/" + fileName, mapData = content };

        string jsonData = JsonUtility.ToJson(requestData, false); 

        UnityWebRequest request = new UnityWebRequest(baseUrl + "/GetMap", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            onSave?.Invoke();
        }
        else
        {
            Debug.LogError("Error fetching movies");
        }
    }
}