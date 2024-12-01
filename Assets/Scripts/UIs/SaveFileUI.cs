using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
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
        fileDescText.text = "로딩중...";
        saveButton.gameObject.SetActive(false);
        loadButton.gameObject.SetActive(false);
        GlobalManager.Instance.GetSaveData(fileName, (data) =>
        {
            if (!GameManager.isInGame) saveButton.gameObject.SetActive(false);
            if (data == null)
            {
                fileDescText.text = "저장 데이터 없음";
                loadButton.gameObject.SetActive(false);
            }
            else
            {
                fileDescText.text = $"플레이 타임: {Utilities.TimeCode(Mathf.RoundToInt(data.timePlayed))}\n마지막 저장: {LastSaveDesc(data.lastSaved)}";
                loadButton.gameObject.SetActive(true);
            }
        });
    }
    public void Load()
    {
        GlobalManager.Instance.LoadGame(fileName);
    }
    public void Save()
    {
        GlobalManager.Instance.Save(fileName);
        Set(fileName);
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