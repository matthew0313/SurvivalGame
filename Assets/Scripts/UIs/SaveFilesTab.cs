using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveFilesTab : MonoBehaviour
{
    [SerializeField] Transform filesAnchor;
    [SerializeField] SaveFileUI prefab;
    public void InstantiateButtons()
    {
        SaveFileUI tmp = Instantiate(prefab, filesAnchor);
        tmp.Set("AutoSave");
        for(int i = 0; i < GlobalManager.Instance.fileCount; i++)
        {
            SaveFileUI tmp2 = Instantiate(prefab, filesAnchor);
            tmp2.Set($"Save{i+1}");
        }
    }
}