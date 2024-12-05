using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(Debugger))]
public class Debugger_Editor : Editor
{
    public VisualTreeAsset treeAsset;
    public override VisualElement CreateInspectorGUI()
    {
        VisualElement element = treeAsset.CloneTree();
        element.Q<Button>("GiveItemToPlayer").clicked += (target as Debugger).GiveItemToPlayer;
        element.Q<Button>("HealPlayer").clicked += (target as Debugger).HealPlayer;
        element.Q<Button>("DamagePlayer").clicked += (target as Debugger).DamagePlayer;
        element.Q<Button>("SetPlayerHp").clicked += (target as Debugger).SetPlayerHp;
        element.Q<Button>("ForceChangeMusic").clicked += (target as Debugger).ForceChangeMusic;
        element.Q<Button>("PlayCutscene").clicked += (target as Debugger).PlayCutscene;
        return element;
    }
}
