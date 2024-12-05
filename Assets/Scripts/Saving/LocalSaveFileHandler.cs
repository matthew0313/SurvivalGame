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
    public override void Save(string data, string fileName, Action onSave = null)
    {
        string path = Path.Combine(savePath, fileName + fileExtension);
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                using (StreamWriter writer = new StreamWriter(stream))
                {
                    writer.Write(data);
                }
            }
            onSave?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError("Save Path:" + path + "\n" + e);
        }
    }
    public override void Load(string fileName, Action<string> onLoad = null)
    {
        string path = Path.Combine(savePath, fileName + fileExtension);
        string loadedData = null;
        if (File.Exists(path))
        {
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        loadedData = reader.ReadToEnd();
                    }
                }
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