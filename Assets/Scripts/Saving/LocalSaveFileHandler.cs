using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System.IO;
using System;

public class LocalSaveFileHandler : SaveFileHandler
{
    readonly string savePath, fileExtension;
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
}