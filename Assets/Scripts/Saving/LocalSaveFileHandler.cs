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









    /*--------------------------------------------서버 연결--------------------------------------------*/

    // 불러오기
    private IEnumerator GetMapData()
    {
        var requestData = new { uid = ""}; // 기기 고유식별번호 넣기
        
        #region 서버 연결
        string jsonData = JsonUtility.ToJson(requestData); // JSON 문자열로 변환

        // UnityWebRequest로 POST 요청 생성
        UnityWebRequest request = new UnityWebRequest(baseUrl + "/LoadMap", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청하기
        yield return request.SendWebRequest();
        #endregion

        // 리턴받은 데이터 배열에 파싱
        string[] mapArray = JsonUtility.FromJson<string[]>(request.downloadHandler.text);
        #region 맵 데이터 리턴형식 (JSON)
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
            // 여기에 맵 데이터 불러온 후 필요한 코드 작성
        }
        else
        {
            Debug.LogError("Error fetching movies: " + mapArray[0]);
        }
    }

    // 저장
    public IEnumerator SaveMapData()
    {
        var requestData = new { uid = "", mapData= "" }; // 기기 고유식별번호 & 맵데이터 JSON 형식 -> string으로 파싱해서 넣기 (최대한 용량 줄이고, 빈공간 없애기)
      
        #region 서버 연결
        string jsonData = JsonUtility.ToJson(requestData); // JSON 문자열로 변환

        // UnityWebRequest로 POST 요청 생성
        UnityWebRequest request = new UnityWebRequest(baseUrl + "/GetMap", "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        // 요청하기
        yield return request.SendWebRequest();

        #endregion

        // 리턴받은 데이터 배열에 파싱
        string resMSG = JsonUtility.FromJson<string>(request.downloadHandler.text);
       
        #region 맵 저장여부 메시지
        /*
            저장 완료 or 저장중 문제가 발생하였습니다.
         */
        #endregion

        if (request.result == UnityWebRequest.Result.Success)
        {
            // 여기에 맵 데이터 불러온 후 필요한 코드 작성
        }
        else
        {
            Debug.LogError("Error fetching movies: " + resMSG);
        }
    }
}