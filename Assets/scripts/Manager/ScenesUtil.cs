using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor;
#endif

public class ScenesUtil : SingletonAutocreate<ScenesUtil>
{

    private bool m_SceneLoaded = false;
    public static bool SceneLoaded
    {
        get
        {
            if (Instance.m_SceneLoaded)
            {
                Instance.m_SceneLoaded = false;
                return true;
            }

            return false;
        }
    }

    private float m_loadTime = 0f;
    public static float LoadingTime
    {
        get { return Instance.m_loadTime; }
    }

    public static void LoadMain()
    {
        UIManager.LoadMainUI(true);
        LoadNewSceneByName("Main");
    }

    public static void LoadLevel(string sceneName)
    {
        UIManager.LoadGameUI();
        LoadNewSceneByName(sceneName);
    }

    public static void LoadNewSceneById(string levelId, bool fadeTitleScreen = true)
    {
        var sceneName = levelId;//TODO

        Instance.StartCoroutine(Instance.CR_LoadNewScene(sceneName, fadeTitleScreen));
    }

    public static void LoadNewSceneByName(string sceneName, bool fadeTitleScreen = true)
    {
        Instance.StartCoroutine(Instance.CR_LoadNewScene(sceneName, fadeTitleScreen));
    }


    private IEnumerator CR_LoadNewScene(string sceneName, bool fadeTitleScreen)
    {
        m_SceneLoaded = false;
        float startTime = Time.time;
        m_loadTime = 0;
        AsyncOperation asyncOperation = LoadNewScene(sceneName, false, true);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }
        m_SceneLoaded = true;
        m_loadTime = Time.time - startTime;
        Debug.Log("Load Scene Time Is :" + m_loadTime);
    }

    public static AsyncOperation LoadNewScene(string sceneName, bool additive = false, bool asyncLoad = false)
    {
        // Save off the name of the last scene loaded
        Scene scene = SceneManager.GetActiveScene();

        var mode = additive ? LoadSceneMode.Additive : LoadSceneMode.Single;

        Application.backgroundLoadingPriority = asyncLoad ? ThreadPriority.Low : ThreadPriority.High;

#if UNITY_EDITOR

        if (asyncLoad)
        {
            return EditorSceneManager.LoadSceneAsync(sceneName, mode);
        }
        else
        {
            EditorSceneManager.LoadScene(sceneName, mode);
        }

#else

        if(asyncLoad){
            return SceneManager.LoadSceneAsync(sceneName,mode);
        }else{
            SceneManager.LoadScene(sceneName,mode);
        }

#endif

        return null;
    }
}
