using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;



[CustomEditor(typeof(GameConfig))]
[CanEditMultipleObjects]
public class GameConfigEditor : Editor
{
    GameConfig m_config;
    private void OnEnable()
    {
        m_config = (GameConfig)target;
    }

    bool backupFile = false;
    Dictionary<string, bool> foldoutDict = new Dictionary<string, bool>();

    public override void OnInspectorGUI()
    {
        this.ScriptLayout(false, true);

        //EditorGUILayout.Space();

        //serializedObject.Update();
        //// ------------------------- Editor View -----------------------------------------

        //serializedObject.ApplyModifiedProperties();

        /*
        // ------------------------- Configs -----------------------------------------
        GUILayout.Space(30);
        int lv = EditorGUI.indentLevel;
        if (IsFoldout("---- Configs ----"))
        {
            EditorGUI.indentLevel = lv + 1;
            GUILayout.Space(30);
            base.OnInspectorGUI();
        }
        EditorGUI.indentLevel = lv;
        */
    }



    #region Common Functions
    private void SaveAssets()
    {
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }
    #endregion

    #region GUILayout Style Lib

    void GUILayout_FoldableBlock(string title, System.Action onGUIContent, float spacing = 10, System.Action onFolded = null)
    {
        if (!foldoutDict.ContainsKey(title)) foldoutDict[title] = false;

        bool isOpen = foldoutDict[title];
        UICustomHelper.Foldout(title, ref isOpen, onGUIContent, spacing);
        foldoutDict[title] = isOpen;

        if (!isOpen && onFolded != null) onFolded(); // 关闭回调
    }


    void GUILayout_HorizontalBlock(System.Action onGUIContent)
    {
        GUILayout.BeginHorizontal();
        if (onGUIContent != null) onGUIContent();
        GUILayout.EndHorizontal();
    }


    void GUIButton(string name, System.Action onClick)
    {
        if (GUILayout.Button(name))
        {
            if (onClick != null) onClick();
        }
    }

    private bool IsFoldout(string key)
    {
        if (!foldoutDict.ContainsKey(key))
        {
            foldoutDict[key] = false;
        }
        foldoutDict[key] = EditorGUILayout.Foldout(foldoutDict[key], key);
        return foldoutDict[key];
    }


    private void GUILayout_ColorBlock(System.Action onGUIContent, Color color)
    {
        var c = GUI.color;
        GUI.color = color;
        if (onGUIContent != null) onGUIContent();
        GUI.color = c;

    }


    #endregion


}



public class UICustomHelper
{
    /// <summary>
    /// 折叠视图
    /// </summary>
    /// <param name="title"></param>
    /// <param name="state"></param>
    /// <param name="onDisplay"></param>
    public static void Foldout(string title, ref bool state, System.Action onDisplay, float spacing = 10)
    {
        GUIStyle headerFoldout = new GUIStyle("ShurikenModuleTitle")
        {
            font = (new GUIStyle("Label")).font,
            fixedHeight = 22,
            fixedWidth = EditorGUIUtility.currentViewWidth - 5,
            border = new RectOffset(15, 15, 4, 4),
            stretchWidth = true,
            // fixedWidth = 0,
            contentOffset = new Vector2(5f, -2f),
        };

        string t = (state ? "▼" : "▶") + " " + title;
        state = EditorGUILayout.Foldout(state, t, true, headerFoldout);

        Color c = GUI.backgroundColor;
        if (state)
        {
            GUILayout.Space(spacing);
            if (onDisplay != null) onDisplay();
            GUILayout.Space(spacing);
        }
        else
        {
            GUILayout.Space(spacing);
        }
    }
}