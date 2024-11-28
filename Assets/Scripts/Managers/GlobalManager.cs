using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System;
public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance { get; private set; }
    SaveFileHandler handler;

    [SerializeField] RectTransform sceneLoadBlack;
    [SerializeField] Text sceneLoadingText;
    [SerializeField] Image brightnessImage;
    [SerializeField] string saveFileName, fileExtension;
    [SerializeField] int m_fileCount = 10;
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
        handler = new LocalSaveFileHandler(Path.Combine(".", saveFileName), fileExtension);
        Settings.onBrightnessChange += () => brightnessImage.color = new Color(0, 0, 0, (1.0f - Settings.brightness) / 2.0f);
        SceneManager.sceneLoaded += OnSceneLoad;
    }
    private void Start()
    {
        LoadSettings();
    }
    public void SaveSettings()
    {
        handler.Save(Settings.Save(), "Settings");
    }
    public void LoadSettings()
    {
        SettingsSaveData data = handler.Load<SettingsSaveData>("Settings");
        if (data == null) data = new();
        Settings.Load(data);
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
        SaveData data = handler.Load<SaveData>(fileName);
        if (data != null)
        {
            foreach (var i in GetSavables()) i.Load(data);
        }
    }
    void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (GameManager.isInGame == false || newGame) return;
        Load(loadingFile);
    }
    public void LoadGame(string fileName)
    {
        if (loadingScene) return;
        newGame = false;
        loadingFile = fileName;
        LoadScene("Game");
    }
    public void NewGame()
    {
        if (loadingScene) return;
        newGame = true;
        LoadScene("Game");
    }
    public SaveData GetSaveData(string fileName) => handler.Load<SaveData>(fileName);

    bool loadingScene = false;
    public void LoadScene(string sceneName)
    {
        if (loadingScene) return;
        loadingScene = true;
        sceneLoadBlack.pivot = new Vector2(1.0f, 0.5f);
        sceneLoadBlack.DOScaleX(1.0f, 0.5f).SetUpdate(true).SetEase(Ease.InCirc).OnComplete(() =>
        {
            sceneLoadingText.gameObject.SetActive(true);
            SceneManager.LoadScene(sceneName);
            sceneLoadingText.gameObject.SetActive(false);
            sceneLoadBlack.pivot = new Vector2(0.0f, 0.5f);
            sceneLoadBlack.DOScaleX(0.0f, 0.5f).SetUpdate(true).SetEase(Ease.InCirc);
            loadingScene = false;
        });
    }
    private void OnApplicationQuit()
    {
        if(GameManager.isInGame && GameManager.Instance.CanSave()) Save("AutoSave");
    }
    IEnumerable<ISavable> GetSavables() => FindObjectsOfType<MonoBehaviour>().OfType<ISavable>();
}