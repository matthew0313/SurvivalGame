using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;
using UnityEngine.Events;
public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance { get; private set; }
    SaveFileHandler handler;
    LocalSaveFileHandler localHandler;
    ServerSaveFileHandler serverHandler;

    [SerializeField] RectTransform sceneLoadBlack;
    [SerializeField] Text sceneLoadingText;
    [SerializeField] Image brightnessImage;

    [Header("Saving")]
    [SerializeField] int m_fileCount = 10;
    [SerializeField] SaveHandlerMode saveMode = SaveHandlerMode.Local;

    [Header("LocalSave")]
    [SerializeField] string fileExtension;
    [SerializeField] string saveFileName;

    [Header("ServerSave")]
    [SerializeField] string baseURL = "survivalgameserver.azurewebsites.net";
    public int fileCount => m_fileCount;
    SaveData data;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        localHandler = new LocalSaveFileHandler(Path.Combine(".", saveFileName), fileExtension);
        serverHandler = new ServerSaveFileHandler(baseURL, this);
        SwitchHandler(saveMode);
        Settings.onBrightnessChange += () => brightnessImage.color = new Color(0, 0, 0, (1.0f - Settings.brightness) / 2.0f);
    }
    public void SwitchHandler(SaveHandlerMode mode)
    {
        switch (mode)
        {
            case SaveHandlerMode.Server: handler = serverHandler; break;
            default: handler = localHandler; break;
        }
    }
    private void Start()
    {
        LoadSettings();
    }
    public void SaveSettings()
    {
        localHandler.Save(Settings.Save(), "Settings");
    }
    public void LoadSettings()
    {
        localHandler.Load<SettingsSaveData>("Settings", (data) =>
        {
            if (data == null) data = new();
            Settings.Load(data);
        });
    }
    public void Save(string fileName)
    {
        if (GameManager.isInGame == false) return;
        SaveData data = new();
        foreach (var i in GetSavables()) i.Save(data);
        handler.Save(data, fileName);
    }
    string loadingFile = "";
    bool newGame = false;
    public void Load(string fileName)
    {
        handler.Load<SaveData>(fileName, (data) =>
        {
            if (data != null)
            {
                foreach (var i in GetSavables()) i.Load(data);
            }
        });
    }
    public void GetSaveData(string fileName, Action<SaveData> onLoad) => handler.Load(fileName, onLoad);

    bool loadingScene = false;
    public void LoadScene(string sceneName)
    {
        if (loadingScene) return;
        loadingScene = true;
        sceneLoadBlack.pivot = new Vector2(1.0f, 0.5f);
        sceneLoadBlack.DOScaleX(1.0f, 0.5f).SetUpdate(true).SetEase(Ease.InCirc).OnComplete(() =>
        {
            sceneLoadingText.gameObject.SetActive(true);
            UnityAction<Scene, LoadSceneMode> onLoad = (a, b) =>
            {
                sceneLoadingText.gameObject.SetActive(false);
                sceneLoadBlack.pivot = new Vector2(0.0f, 0.5f);
                sceneLoadBlack.DOScaleX(0.0f, 0.5f).SetUpdate(true).SetEase(Ease.InCirc);
                loadingScene = false;
            };
            onLoad += (a, b) => SceneManager.sceneLoaded -= onLoad;
            SceneManager.sceneLoaded += onLoad;
            SceneManager.LoadScene(sceneName);
        });
    }
    public void LoadGame(string loadingFileName)
    {
        if (loadingScene) return;
        loadingScene = true;
        sceneLoadBlack.pivot = new Vector2(1.0f, 0.5f);
        sceneLoadBlack.DOScaleX(1.0f, 0.5f).SetUpdate(true).SetEase(Ease.InCirc).OnComplete(() =>
        {
            sceneLoadingText.gameObject.SetActive(true);
            UnityAction<Scene, LoadSceneMode> onLoad = (a, b) =>
            {
                if (loadingFileName != null)
                {
                    handler.Load<SaveData>(loadingFileName, (data) =>
                    {
                        if (data != null)
                        {
                            foreach (var i in GetSavables()) i.Load(data);
                        }
                        sceneLoadingText.gameObject.SetActive(false);
                        sceneLoadBlack.pivot = new Vector2(0.0f, 0.5f);
                        sceneLoadBlack.DOScaleX(0.0f, 0.5f).SetUpdate(true).SetEase(Ease.InCirc);
                        loadingScene = false;
                    });
                }
                else
                {
                    sceneLoadingText.gameObject.SetActive(false);
                    sceneLoadBlack.pivot = new Vector2(0.0f, 0.5f);
                    sceneLoadBlack.DOScaleX(0.0f, 0.5f).SetUpdate(true).SetEase(Ease.InCirc);
                    loadingScene = false;
                }
            };
            onLoad += (a, b) => SceneManager.sceneLoaded -= onLoad;
            SceneManager.sceneLoaded += onLoad;
            SceneManager.LoadScene("Game");
        });
    }
    public void NewGame()
    {
        LoadGame(null);
    }
    private void OnApplicationQuit()
    {
        if(GameManager.isInGame && GameManager.Instance.CanSave()) Save("AutoSave");
    }
    IEnumerable<ISavable> GetSavables() => FindObjectsOfType<MonoBehaviour>().OfType<ISavable>();
}
[System.Serializable]
public enum SaveHandlerMode
{
    Local,
    Server
}