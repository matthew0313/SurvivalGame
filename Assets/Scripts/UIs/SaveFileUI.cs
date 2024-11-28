using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveFileUI : MonoBehaviour
{
    [SerializeField] Text fileNameText, fileDescText;
    [SerializeField] Button saveButton, loadButton;
    string fileName;
    public void Set(string fileName)
    {
        this.fileName = fileName;
        fileNameText.text = fileName;
        SaveData data = GlobalManager.Instance.GetSaveData(fileName);
        if (data == null)
        {
            fileDescText.text = "저장 데이터 없음";
            loadButton.gameObject.SetActive(false);
        }
        else
        {
            fileDescText.text = $"플레이 타임: {Mathf.RoundToInt(data.timePlayed) / 3600}:{Mathf.RoundToInt(data.timePlayed) % 3600 / 60}:{Mathf.RoundToInt(data.timePlayed) % 60}\n마지막 저장: {LastSaveDesc(data.lastSaved)}";
        }
    }
    void Save()
    {
        if (GameManager.isInGame) GlobalManager.Instance.SaveGame(fileName);
    }
    string LastSaveDesc(long lastSaveTick)
    {
        long tickPast = DateTime.Now.Ticks - lastSaveTick;
        int secondsPast = (int)tickPast / 1000000;
        if (secondsPast <= 300) return "방금 전";
        else if (secondsPast <= 3600) return $"{secondsPast / 60}분 전";
        else if (secondsPast <= 3600 * 24) return $"{secondsPast / 3600}시간 전";
        else
        {
            return $"{secondsPast / (3600 * 24)}일 전";
        }
    }
}
