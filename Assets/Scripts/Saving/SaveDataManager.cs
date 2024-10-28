using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Data.Common;
public class SaveDataManager : MonoBehaviour
{
    public static SaveDataManager Instance { get; private set; }
    SaveFileHandler handler;

    [SerializeField] string saveFileName, fileExtension;
    SaveData data;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
        handler = new LocalSaveFileHandler(Path.Combine(Application.persistentDataPath, saveFileName), fileExtension);
    }
    private void Start()
    {
        LoadGame(1);
    }
    public void SaveGame(int fileNumber)
    {
        SaveData data = new();
        foreach (var i in GetSavables()) i.Save(data);
        handler.Save(data, "Save" + fileNumber);
    }
    public void LoadGame(int fileNumber)
    {
        SaveData data = handler.Load("Save" + fileNumber);
        if (data != null)
        {
            foreach (var i in GetSavables()) i.Load(data);
        }
    }
    private void OnApplicationQuit()
    {
        SaveGame(1);
    }
    IEnumerable<ISavable> GetSavables() => FindObjectsOfType<MonoBehaviour>().OfType<ISavable>();
}