using System.Collections;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Networking;

public class LocalSaveFileHandler : SaveFileHandler
{
    readonly string savePath, fileExtension;
    private const string baseUrl = "survivalgameserver.azurewebsites.net";
    public LocalSaveFileHandler(string savePath, string fileExtension)
    {
        this.savePath = savePath;
        this.fileExtension = fileExtension;
    }
    public override void Save<T>(T data, string fileName) where T : class
    {
        string path = Path.Combine(savePath, fileName + fileExtension);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            string dataToStore = JsonUtility.ToJson(data, true);
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Save Path:" + path + "\n" + e);
        }
    }
    public override T Load<T>(string fileName) where T : class
    {
        string path = Path.Combine(savePath, fileName + fileExtension);
        T loadedData = null;
        if (File.Exists(path))
        {
            try
            {
                string dataToLoad;
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        dataToLoad = reader.ReadToEnd();
                    }
                }
                loadedData = JsonUtility.FromJson<T>(dataToLoad);
            }
            catch (Exception e)
            {
                Debug.LogError("Load Path:" + path + "\n" + e);
            }
        }
        return loadedData;
    }
    public override void Delete(string fileName)
    {
        string path = Path.Combine(savePath, fileName + fileExtension);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }









    /*--------------------------------------------���� ����--------------------------------------------*/

    // �ҷ�����
    private IEnumerator GetMapData()
    {
        var requestData = new { uid = ""}; // ��� �����ĺ���ȣ �ֱ�
        
        #region ���� ����
        string jsonData = JsonUtility.ToJson(requestData); // JSON ���ڿ��� ��ȯ

        // UnityWebRequest�� POST ��û ����
        UnityWebRequest request = new UnityWebRequest(baseUrl + "/LoadMap", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // ��û�ϱ�
        yield return request.SendWebRequest();
        #endregion

        // ���Ϲ��� ������ �迭�� �Ľ�
        string[] mapArray = JsonUtility.FromJson<string[]>(request.downloadHandler.text);
        #region �� ������ �������� (JSON)
            /*
             [
                "test1234",
                "test1234",
                "test1234",
            ]
             */
            #endregion

        if (request.result == UnityWebRequest.Result.Success)
        {
            // ���⿡ �� ������ �ҷ��� �� �ʿ��� �ڵ� �ۼ�
        }
        else
        {
            Debug.LogError("Error fetching movies: " + mapArray[0]);
        }
    }

    // ����
    public IEnumerator SaveMapData()
    {
        var requestData = new { uid = "", mapData= "" }; // ��� �����ĺ���ȣ & �ʵ����� JSON ���� -> string���� �Ľ��ؼ� �ֱ� (�ִ��� �뷮 ���̰�, ����� ���ֱ�)
      
        #region ���� ����
        string jsonData = JsonUtility.ToJson(requestData); // JSON ���ڿ��� ��ȯ

        // UnityWebRequest�� POST ��û ����
        UnityWebRequest request = new UnityWebRequest(baseUrl + "/GetMap", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // ��û�ϱ�
        yield return request.SendWebRequest();

        #endregion

        // ���Ϲ��� ������ �迭�� �Ľ�
        string resMSG = JsonUtility.FromJson<string>(request.downloadHandler.text);
       
        #region �� ���忩�� �޽���
        /*
            ���� �Ϸ� or ������ ������ �߻��Ͽ����ϴ�.
         */
        #endregion

        if (request.result == UnityWebRequest.Result.Success)
        {
            // ���⿡ �� ������ �ҷ��� �� �ʿ��� �ڵ� �ۼ�
        }
        else
        {
            Debug.LogError("Error fetching movies: " + resMSG);
        }
    }
}