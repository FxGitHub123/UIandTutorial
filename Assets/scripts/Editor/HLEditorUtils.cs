using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class HLEditorUtils
{
    #region 编辑器布局

    public static void GUIDisable(bool disable, Action action)
    {
        var old = GUI.enabled;
        GUI.enabled = !disable;

        if (action != null) action.Invoke();

        GUI.enabled = old;
    }

    public static void GUIColor(Color contentColor, Color backgroundColor, Action action)
    {
        var oldContentColor = GUI.contentColor;
        var oldBackgroundColor = GUI.backgroundColor;

        GUI.contentColor = contentColor;
        GUI.backgroundColor = backgroundColor;

        if (action != null) action.Invoke();

        GUI.contentColor = oldContentColor;
        GUI.backgroundColor = oldBackgroundColor;
    }

    public static void GUIColor(Color color, Action action)
    {
        var oldColor = GUI.color;

        GUI.color = color;

        if (action != null) action.Invoke();

        GUI.color = oldColor;
    }

    public static void PropertyPopup(this SerializedProperty property, string[] popEnum = null)
    {
        if (popEnum != null)
        {
            int index = -1;
            for (int i = 0; i < popEnum.Length; i++)
            {
                if (popEnum[i] == property.stringValue)
                {
                    index = i;
                    break;
                }
            }
            index = EditorGUILayout.Popup(property.displayName, index, popEnum);
            if (index >= 0 && index < popEnum.Length)
            {
                property.stringValue = popEnum[index];
            }
        }
        else
        {
            EditorGUILayout.PropertyField(property, true);
        }
    }

    public static SerializedProperty PropertyLayout(this Editor target, string propertyName, string[] popEnum = null)
    {
        var property = target.serializedObject.FindProperty(propertyName);
        if (property == null)
        {
            Debug.LogWarningFormat("No property {0} for {1}", propertyName, target.target);
            return null;
        }
        property.PropertyPopup(popEnum);
        return property;
    }

    public static SerializedProperty PropertyLayout(this SerializedProperty target, string propertyName, string[] popEnum = null)
    {
        var property = target.FindPropertyRelative(propertyName);
        if (property == null)
        {
            Debug.LogWarningFormat("No property {0} for {1}", propertyName, target);
            return null;
        }
        property.PropertyPopup(popEnum);
        return property;
    }

    public static void ScriptLayout(MonoBehaviour target)
    {
        if (target == null) return;
        GUI.enabled = false;
        var script = MonoScript.FromMonoBehaviour(target);
        EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), true);
        GUI.enabled = true;
    }

    public static void ScriptLayout(this ScriptableObject target)
    {
        if (target == null) return;
        GUI.enabled = false;
        var script = MonoScript.FromScriptableObject(target);
        EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), true);
        GUI.enabled = true;
    }

    public static void ScriptLayout(this Editor target, bool includeEditorScript = false, bool includeData = false)
    {
        GUI.enabled = false;

        var monoScript = target.target as MonoBehaviour;
        var scriptObjectScript = target.target as ScriptableObject;
        var script = monoScript ? MonoScript.FromMonoBehaviour(monoScript) : MonoScript.FromScriptableObject(scriptObjectScript);
        EditorGUILayout.ObjectField("Script", script, typeof(MonoScript), true);

        if (includeEditorScript)
        {
            EditorGUILayout.ObjectField("Edirot Script", MonoScript.FromScriptableObject(target), typeof(MonoScript), true);
        }

        if (includeData && scriptObjectScript)
        {
            EditorGUILayout.ObjectField("Data", scriptObjectScript, scriptObjectScript.GetType(), true);
        }
        GUI.enabled = true;
    }

    #endregion

    #region 执行进度条
    static string m_progressTitle = null;
    static string m_progressInfo = null;

    public static bool DisplayCancelableProgressBar(float progressStart, float progressEnd, float subProgress, string info = null, string title = null)
    {
        if (title != null) m_progressTitle = title;
        if (info != null) m_progressInfo = info;

        if (m_progressTitle == null || progressStart > 1 || progressEnd > 1 || progressStart >= progressEnd || subProgress > 1)
        {
            Debug.LogWarning("ProgressBar params error");
            m_progressTitle = null;
            EditorUtility.ClearProgressBar();
            return false;
        }

        var rate = progressStart + (progressEnd - progressStart) * subProgress;
        if (rate >= 1)
        {
            m_progressTitle = null;
            EditorUtility.ClearProgressBar();
            return false;
        }
        if (EditorUtility.DisplayCancelableProgressBar(m_progressTitle, string.Format("{0} ({1:F2}%)", m_progressInfo, subProgress * 100), rate))
        {
            EditorUtility.ClearProgressBar();
            return true;
        }
        return false;
    }

    #endregion

    public static bool ErrorAlert(string title, string info, bool ignoreError = false)
    {
        if (ignoreError)
        {
            Debug.LogWarningFormat("{0} : {1}", title, info);
            return true;
        }

        return EditorUtility.DisplayDialog(title, info, "OK");
    }

    #region 场景过滤

    public static void ClearSearchFilter()
    {
        SetSearchFilter("", SearchableEditorWindow.SearchMode.All);
    }

    public static void SetSearchFilter(Type type)
    {
        SetSearchFilter(type.ToString(), SearchableEditorWindow.SearchMode.Type);
    }

    public static void SetSearchFilter(string filter, SearchableEditorWindow.SearchMode filterMode)
    {
        SearchableEditorWindow hierarchy = null;
        SearchableEditorWindow[] windows = Resources.FindObjectsOfTypeAll<SearchableEditorWindow>();

        foreach (SearchableEditorWindow window in windows)
        {
            if (window.GetType().ToString() == "UnityEditor.SceneHierarchyWindow")
            {
                hierarchy = window;
                break;
            }
        }

        if (hierarchy == null) return;

        MethodInfo setSearchType = typeof(SearchableEditorWindow).GetMethod("SetSearchFilter", BindingFlags.NonPublic | BindingFlags.Instance);
        object[] parameters = new object[] { filter, filterMode, false, false };

        setSearchType.Invoke(hierarchy, parameters);
    }

    #endregion

    public static string GetScenePath(string sceneName, bool matchFullName = true)
    {
        if (string.IsNullOrEmpty(sceneName)) return "";

        var ids = AssetDatabase.FindAssets(sceneName + " t:Scene");

        foreach(var id in ids)
        {
            var path = AssetDatabase.GUIDToAssetPath(id);
            if (!matchFullName || path.EndsWith(sceneName + ".unity"))
            {
                return path;
            }
        }

        return "";
    }
}
