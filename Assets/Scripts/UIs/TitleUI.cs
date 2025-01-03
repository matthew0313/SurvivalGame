using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DG.Tweening;

public class TitleUI : MonoBehaviour
{
    [SerializeField] Image introBack;
    [SerializeField] Text introText;
    [SerializeField] RectTransform newButton, loadButton, settingsButton;
    [SerializeField] Slider volumeSlider, brightnessSlider;
    [SerializeField] AudioSource titleMusic;
    [SerializeField] SaveFilesTab saveFiles;
    static bool introWatched = false;
    private void Awake()
    {
        if (!introWatched)
        {
            introWatched = true;
            introText.DOColor(new Color(1, 1, 1, 1), 1.0f).OnComplete(() =>
            {
                introText.DOColor(new Color(1, 1, 1, 0), 1.0f).SetDelay(1.0f).OnComplete(() =>
                {
                    titleMusic.Play();
                    introBack.DOColor(new Color(0, 0, 0, 0), 3.0f).OnComplete(() =>
                    {
                        AfterIntro();
                    });
                });
            });
        }
        else
        {
            introBack.gameObject.SetActive(false);
            introText.gameObject.SetActive(false);
            titleMusic.Play();
            AfterIntro();
        }
        saveFiles.InstantiateButtons();
        Settings.onMasterVolumeChange += OnMasterVolumeChange;
        Settings.onBrightnessChange += OnBrightnessChange;
        OnMasterVolumeChange(); OnBrightnessChange();
        volumeSlider.onValueChanged.AddListener((float value) => Settings.masterVolume = value);
        brightnessSlider.onValueChanged.AddListener((float value) => Settings.brightness = value);
    }
    void AfterIntro()
    {
        newButton.DOAnchorPosX(0.0f, 2.0f).SetEase(Ease.OutCirc);
        loadButton.DOAnchorPosX(0.0f, 2.0f).SetDelay(0.5f).SetEase(Ease.OutCirc);
        settingsButton.DOAnchorPosX(0.0f, 2.0f).SetDelay(1.0f).SetEase(Ease.OutCirc);
    }
    void OnMasterVolumeChange()
    {
        titleMusic.volume = Settings.masterVolume;
        volumeSlider.value = Settings.masterVolume;
    }
    void OnBrightnessChange()
    {
        brightnessSlider.value = Settings.brightness;
    }
    private void OnDestroy()
    {
        Settings.onMasterVolumeChange -= OnMasterVolumeChange;
        Settings.onBrightnessChange -= OnBrightnessChange;  
    }
    public void NewGame()
    {
        GlobalManager.Instance.NewGame();
    }
    public void SaveSettings() => GlobalManager.Instance.SaveSettings();
    public void LoadSettings() => GlobalManager.Instance.LoadSettings();
}