using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialManager : SingletonAutocreate<TutorialManager>
{
    #region 引导面板
    private List<TutorialObj> m_tutorialObjList;

    private UITutorialPanel m_tutorialPanel;

    public TutorialObj GetTutorialObj(string name)
    {
        foreach (var item in m_tutorialObjList)
        {
            if (item.targetName.Equals(name))
            {
                return item;
            }
        }

        return null;
    }

    public void DisPose(TutorialObj obj)
    {
        if (m_tutorialObjList != null && m_tutorialObjList.Contains(obj)) m_tutorialObjList.Remove(obj);
    }

    public void Register(TutorialObj obj)
    {
        if (m_tutorialObjList == null) m_tutorialObjList = new List<TutorialObj>();
        if (m_tutorialObjList.Contains(obj)) return;
        foreach (var item in m_tutorialObjList)
        {
            if (item.targetName.Equals(obj.targetName))
            {
                m_tutorialObjList.Remove(item);
                break;
            }
        }
        m_tutorialObjList.Add(obj);
    }

    private void SetTarget(TutorialObj target, bool isMask = true)
    {
        if (m_tutorialPanel == null) m_tutorialPanel = UIManager.GetUIPanel<UITutorialPanel>();
        m_tutorialPanel.OpenPanel();
        m_tutorialPanel.SetGuideTarget(target, isMask);

    }

    public static void SetGuideTarget(TutorialObj target, bool isMask = true)
    {
        Instance.SetTarget(target, isMask);
    }

    public static void SetGuideTarget(string name, bool isMask = true)
    {
        TutorialObj target = Instance.GetTutorialObj(name);
        Instance.SetTarget(target, isMask);
    }

    public void CloseGuide()
    {
        if (m_tutorialPanel == null) m_tutorialPanel = UIManager.GetUIPanel<UITutorialPanel>();
        m_tutorialPanel.CloseGuide();
        //m_tutorialPanel.ClosePanel();
    }

    public void OpenClickMask()
    {
        UIManager.ShowPanel<UITutorialPanel>();
        if (m_tutorialPanel == null) m_tutorialPanel = UIManager.GetUIPanel<UITutorialPanel>();
        //if (m_tutorialPanel.Status == UIPaneStatus.Closed) ;
        m_tutorialPanel.OpenClickMask();
    }

    public void CloseClickMask()
    {
        if (m_tutorialPanel == null) m_tutorialPanel = UIManager.GetUIPanel<UITutorialPanel>();
        m_tutorialPanel.CloseClickMask();
    }

    #endregion

    #region 引导数据
    public const string K_TUTORIAL_SAVE_DATA = "tutorial_save_data";
    private const string m_saveDataFileName = "TutorialData.dat";
    public static string TutorialDataFilePath
    {
        get
        {
            return Path.Combine(Application.persistentDataPath, m_saveDataFileName);
        }
    }
    public const string K_TUTORIAL_ANALYTICS = "tutorial_analytics";
    public string K_TUTORIAL_VERSION = "1.0.0";

    [SerializeField]
    private List<string> m_finishedSteps = new List<string>();
    //private Dictionary<string, TutorialBase> m_tutorials = new Dictionary<string, TutorialBase>();
    private List<int> m_tutotialAnalyticsList;

    [SerializeField]
    private bool m_skiped = false;

    /// <summary>
    /// 根据教程列表来监测任务的完成状态
    /// </summary>
    public static bool IsAllCompleted
    {
        get
        {
            if (Instance == null) return true;
            return false;
            //return Instance.m_finishedSteps.Count >= Global.TutorialConfig.itemList.Count;
        }
    }

    private bool isStrengthLess;
    public static bool IsStrengthLess
    {
        get
        {
            return Instance.isStrengthLess;
        }

        set
        {
            Instance.isStrengthLess = value;
        }
    }

    public void SelectTutorials()
    {

        if (IsAllCompleted)
        {
            // 初始化时, 全步骤已经完成则跳过
            return;
        }



        //foreach (var config in Global.TutorialConfig.itemList)
        //{
        //    if (IsStepFinished(config.id)) continue;

        //    TutorialBase temp = null;
        //    m_tutorials.TryGetValue(config.id, out temp);
        //    if (temp == null)
        //    {
        //        Type type = System.Type.GetType("Tutorial" + config.name);
        //        if (type == null) continue;

        //        temp = GetComponent(type) as TutorialBase;
        //        if (temp == null)
        //        {
        //            temp = gameObject.AddComponent(type) as TutorialBase;
        //            if (temp == null) continue;

        //            m_tutorials[config.id] = temp;
        //        }

        //        temp.Init(config);
        //    }

        //    if (temp != null && temp.IsAvailable() && !temp.IsRunning())
        //    {
        //        temp.Run();
        //    }
        //}
    }

    public static bool IsStepFinished(string tutorialId)
    {
        if (Instance != null)
        {
            return Instance.m_finishedSteps.Contains(tutorialId);
        }
        return false;
    }
    public static void FinishTutorial(string tutorialId)
    {
        if (Instance == null) return;

        Instance.OnTutorialStepEnd(tutorialId);
        Instance.SelectTutorials();
    }

    void OnTutorialStepEnd(string id)
    {
        if (m_finishedSteps.Contains(id)) return;
        m_finishedSteps.Add(id);

        //m_tutorials.Remove(id);

        //定点存储教程步骤
        SaveData();

        //if (IsAllCompleted) GlobalEvents.OnAllTutorialFinished();

    }

    public void SaveData()
    {
        string raw = ToString();
        if (!string.IsNullOrEmpty(raw))
        {
            File.WriteAllText(TutorialDataFilePath, raw);
            //PlayerPrefs.SetString(K_TUTORIAL_SAVE_DATA, raw);
            //PlayerPrefs.Save();
        }
    }

    public void LoadData(string raw = null, string analytics = null)
    {
        // 第一次,尝试本地加载
        if (string.IsNullOrEmpty(raw))
        {
            if (File.Exists(TutorialDataFilePath))
            {
                raw = File.ReadAllText(TutorialDataFilePath);
                //raw = PlayerPrefs.GetString(K_TUTORIAL_SAVE_DATA, "");
                Debug.Log("<color=red>本地加载教程</color>");
                Debug.Log(raw);
                analytics = PlayerPrefs.GetString(K_TUTORIAL_ANALYTICS, "");
            }

        }

        // 第二次, 尝试初始化数据
        if (string.IsNullOrEmpty(raw))
        {
            Debug.Log("<color=red>初始化教程</color>");
            m_finishedSteps.Clear(); //默认初始值
        }
        else
        {
            m_finishedSteps = GetFinishedTutorialSteps(raw);
            if (m_finishedSteps == null)
            {
                m_finishedSteps = new List<string>();
                Debug.Log("<color=red>Tutorial Version Error</color>");
                FinishAllStep();
            }
        }

        //教程埋点
        if (string.IsNullOrEmpty(analytics))
        {
            m_tutotialAnalyticsList = new List<int>();
        }
        else
        {
            m_tutotialAnalyticsList = GetFinishedTutorialAnalytics(analytics);
        }
    }

    public List<string> GetFinishedTutorialSteps(string dataString)
    {
        string[] version = dataString.Split(':');
        var stepStr = version.Length > 1 ? version[1] : null;
        m_skiped = version.Length > 2 ? Convert.ToInt32(version[2]) != 0 : false;

        if (!string.IsNullOrEmpty(version[0]))
        {
            if (!version[0].Equals(K_TUTORIAL_VERSION))
            {
                m_skiped = true;
                return null;
            }
        }
        else
        {
            return null;
        }


        List<string> steps = new List<string>();
        if (!string.IsNullOrEmpty(stepStr))
        {
            string[] raw = stepStr.Split('_');
            steps.AddRange(raw);
        }

        return steps;
    }

    public static void FinishAllStep()
    {
        if (Instance == null) return;

        //foreach (var config in Global.TutorialConfig.itemList)
        //{
        //    Instance.OnTutorialStepEnd(config.id);
        //    Instance.Dispose(config.id);
        //}

        //IsGadgetUnlock = true;
    }

    public List<int> GetFinishedTutorialAnalytics(string data)
    {
        List<int> analytics = new List<int>();
        if (!string.IsNullOrEmpty(data))
        {
            string[] raw = data.Split('_');
            int id = 0;
            for (int i = 0; i < raw.Length; i++)
            {
                id = 0;
                int.TryParse(raw[i], out id);
                analytics.Add(id);
            }
            analytics.Sort((a, b) => {
                if (a > b) return 1;
                else if (a < b) return -1;
                return 0;
            });
        }
        return analytics;

    }

    void Dispose(string tutorialId)
    {
        //TutorialBase tutorialBase = null;
        //if (m_tutorials.TryGetValue(tutorialId, out tutorialBase))
        //{
        //    Destroy(tutorialBase);
        //    m_tutorials.Remove(tutorialId);
        //}
    }

    public override string ToString()
    {
        return FinishedStepsToString();
    }
    public string FinishedStepsToString()
    {
        string buff = K_TUTORIAL_VERSION + ":";

        if (m_finishedSteps != null)
        {
            for (int i = 0; i < m_finishedSteps.Count; i++)
            {
                buff += m_finishedSteps[i] + ((i < m_finishedSteps.Count - 1) ? "_" : "");
            }
        }
        buff += m_skiped ? ":1" : ":0";
        return buff;
    }
    #endregion
}
