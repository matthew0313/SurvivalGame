using System.Collections;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Networking;

public class LocalSaveFileHandler : SaveFileHandler
{
    readonly string savePath, fileExtension;
    public LocalSaveFileHandler(string savePath, string fileExtension)
    {
        this.savePath = savePath;
        this.fileExtension = fileExtension;
    }
    public override void Save<T>(T data, string fileName, Action<T> onSave = null) where T : class
    {
        string path = Path.Combine(savePath, fileName + fileExtension);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            string dataToStore = JsonUtility.ToJson(data, false);
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(dataToStore);
                }
            }
            onSave?.Invoke(data);
        }
        catch (Exception e)
        {
            Debug.LogError("Save Path:" + path + "\n" + e);
        }
    }
    public override void Load<T>(string fileName, Action<T> onLoad = null) where T : class
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
        onLoad?.Invoke(loadedData);
    }
    public override void Delete(string fileName, Action onDelete = null)
    {
        string path = Path.Combine(savePath, fileName + fileExtension);
        if (File.Exists(path))
        {
            File.Delete(path);
            onDelete?.Invoke();
        }
    }
}