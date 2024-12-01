using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System;
using UnityEngine.Networking;

public class ServerSaveFileHandler : SaveFileHandler
{
    readonly string baseUrl;
    readonly MonoBehaviour coroutinePlayer;
    public ServerSaveFileHandler(string baseUrl, MonoBehaviour coroutinePlayer)
    {
        this.baseUrl = baseUrl;
        this.coroutinePlayer = coroutinePlayer;
    }
    public override void Load<T>(string fileName, Action<T> onLoad = null)
    {
        coroutinePlayer.StartCoroutine(LoadDataFromServer(fileName, (tmp) => onLoad?.Invoke(JsonUtility.FromJson<T>(tmp))));
    }
    public override void Save<T>(T data, string fileName, Action<T> onSave = null)
    {
        coroutinePlayer.StartCoroutine(SaveDataToServer(fileName, JsonUtility.ToJson(data), () => onSave?.Invoke(data)));
    }
    public override void Delete(string fileName, Action onDelete = null)
    {
        throw new NotImplementedException();
    }
    private IEnumerator LoadDataFromServer(string fileName, Action<string> onLoad)
    {
        WWWForm form = new WWWForm();
        form.AddField("uid", "test");


        using (UnityWebRequest www = UnityWebRequest.Post($"{baseUrl}/LoadMap", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string responseText = www.downloadHandler.text;
                onLoad?.Invoke(responseText);
            }
            else
            {
                Debug.LogError($"Error fetching map data: {www.error}\nResponse: {www.downloadHandler.text}");
            }
        }
    }
    /*private IEnumerator LoadDataFromServer(string fileName, Action<string> onLoad)
    {
        var requestData = new { uid = "test" };

        string jsonData = JsonUtility.ToJson(requestData);

        UnityWebRequest request = new UnityWebRequest(baseUrl + "/LoadMap", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            onLoad?.Invoke(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Error fetching movies" + request.downloadHandler.text);
        }
    }*/

    // ����
    public IEnumerator SaveDataToServer(string fileName, string content, Action onSave)
    {
        var requestData = new { uid = SystemInfo.deviceUniqueIdentifier + "/" + fileName, mapData = content };

        string jsonData = JsonUtility.ToJson(requestData); 

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