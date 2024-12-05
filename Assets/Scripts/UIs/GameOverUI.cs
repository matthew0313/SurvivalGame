using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] Image gameOverButton;
    [SerializeField] Sound gameOverMusic;
    [SerializeField] AudioSource gameOverMusicPlayer;
    private void Start()
    {
        gameOverMusicPlayer.clip = gameOverMusic.clip;
        gameOverMusicPlayer.volume = gameOverMusic.volume * Settings.masterVolume;
        gameOverMusicPlayer.Play();
        gameOverButton.rectTransform.DOAnchorPosY(50.0f, 1.0f).SetDelay(2.0f).SetEase(Ease.OutCirc);
        gameOverButton.DOColor(new Color(1, 1, 1, 1), 1.0f).SetDelay(2.0f);
    }
    public void ReturnToTitle()
    {
        GlobalManager.Instance.LoadScene("Title");
    }
}
