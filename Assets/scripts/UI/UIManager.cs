using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIManager : SingletonAutocreate<UIManager>
{

    [SerializeField]
    private Transform UIMainRoot;
    [SerializeField]
    private Transform UIGameRoot;
    [SerializeField]
    private Transform URoot;

    [SerializeField]
    public UIPrefab[] _allPrefabs;

    private List<UIPanelBase> m_MainPanelStack = new List<UIPanelBase>();
    private List<UIPanelBase> m_GamePanelStack = new List<UIPanelBase>();
    private List<UIPanelBase> m_RootPanelStack = new List<UIPanelBase>();

    private List<UIPanelBase> m_currentList;

    private const string K_UIGAMEPATH = "Canvas/UIGame";
    private const string K_UIMAINPATH = "Canvas/UIMain";
    private Dictionary<string, GameObject> m_PanelDic;
    private TitleScreen m_titleScreen;
    private UITips m_tipsPanel;


    #region 生命周期
    protected override void Awake()
    {
        base.Awake();
        //GlobalEvents.OnGameStatusChanged += OnGameStatusChanged;
        UIMessage.OnUIPanelClose += OnUIPanelClose;
        UIMessage.OnUIPanelOpen += OnUIPanelOpen;
        DontDestroyOnLoad(this);

    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        //GlobalEvents.OnGameStatusChanged -= OnGameStatusChanged;
        UIMessage.OnUIPanelClose -= OnUIPanelClose;
        UIMessage.OnUIPanelOpen -= OnUIPanelOpen;
    }

    private void Start()
    {
        Init();
    }
    public void Init()
    {
        ShowSubPanel<TitleScreen>();
    }

    #endregion


    #region 静态接口
    public static void ShowPanel<T>() where T : UIPanelBase
    {
        Instance.ShowSubPanel<T>();
    }

    public static void ClosePanel<T>() where T : UIPanelBase
    {
        Instance.CloseSubPanel<T>();
    }
    public static T GetUIPanel<T>() where T : UIPanelBase
    {
        return Instance.GetSubPanel<T>();
    }

    public static UIPanelBase GetCurrentPanel()
    {
        return Instance.GetFirstPanel();
    }

    public static void LoadGameUI()
    {
        Instance.CleanPrefabs(UIParentType.Main);//清除Main节点下ui
        //ShowPanel<UIGamePanel>();//加载uigame
        Instance.StartCoroutine(Instance.CR_WaitLoadLevel());
    }

    public static void LoadMainUI(bool fromGameLevel = true)
    {
        Instance.CleanPrefabs(UIParentType.Game);//清除Game节点下ui
        if (fromGameLevel)
        {
            ShowPanel<UIMainButtons>();
            Instance.StartCoroutine(Instance.CR_WaitLoadMainScene());
        }
        else
        {
            ShowPanel<UIMainButtons>();
            ShowPanel<UITutorialPanel>();
        }
    }

    #endregion


    #region 私有方法
    private void ShowSubPanel<T>() where T : UIPanelBase
    {
        string name = typeof(T).Name;
        if (m_PanelDic == null) m_PanelDic = new Dictionary<string, GameObject>();
        if (!m_PanelDic.ContainsKey(name))
        {
            foreach (var item in _allPrefabs)
            {
                GameObject prefab = item.prefab;
                var prefabComp = prefab.GetComponent<T>();

                if (prefabComp != null)
                {
                    prefabComp.type = item.type;
                    Transform parent = null;
                    switch (item.type)
                    {
                        case UIParentType.Root:
                            parent = URoot;
                            break;
                        case UIParentType.Main:
                            parent = UIMainRoot;
                            break;
                        case UIParentType.Game:
                            parent = UIGameRoot;
                            break;
                        default:
                            break;
                    }
                    GameObject clone = Instantiate<GameObject>(prefab, new Vector3(0, 0, 0), Quaternion.identity, parent);//克隆
                    clone.transform.localPosition = new Vector3(0, 0, 0);
                    clone.gameObject.SetActive(false);
                    m_PanelDic.Add(name, clone);
                }

            }
        }

        UIPanelBase panel = m_PanelDic[name].GetComponent<T>();
        if (!m_PanelDic[name].activeSelf)
        {
            m_PanelDic[name].SetActive(true);

        }
        panel.Init();
        panel.OpenPanel();
    }


    private void CloseSubPanel<T>() where T : UIPanelBase
    {
        string name = typeof(T).Name;
        if (m_PanelDic.ContainsKey(name))
        {
            UIPanelBase panel = m_PanelDic[name].GetComponent<T>();
            panel.ClosePanel();
        }
    }

    private void OrderSiblingIndex()
    {
        for (int i = 0; i < m_currentList.Count; i++)
        {
            m_currentList[i].transform.SetSiblingIndex(i);
        }
    }


    private T GetSubPanel<T>() where T : UIPanelBase
    {
        foreach (var item in m_PanelDic)
        {
            var prefabComp = item.Value.GetComponent<T>();
            if (prefabComp != null) return prefabComp;
        }
        return null;
    }

    private UIPanelBase GetFirstPanel()
    {
        if (m_MainPanelStack == null || m_MainPanelStack.Count == 0) Debug.LogError("No UI");
        return m_MainPanelStack[m_MainPanelStack.Count - 1];
    }

    public void CleanPrefabs(UIParentType type)
    {
        UIPanelBase[] panels = null;
        switch (type)
        {
            case UIParentType.Main:
                panels = UIMainRoot.GetComponentsInChildren<UIPanelBase>();
                break;
            case UIParentType.Game:
                panels = UIGameRoot.GetComponentsInChildren<UIPanelBase>();
                break;
            default:
                break;
        }

        foreach (var item in panels)
        {
            item.ClosePanel();
        }
    }

    private void PushStack(UIPanelBase uIPanel)
    {
        switch (uIPanel.type)
        {
            case UIParentType.Root:
                m_currentList = m_RootPanelStack;
                break;
            case UIParentType.Main:
                m_currentList = m_MainPanelStack;
                break;
            case UIParentType.Game:
                m_currentList = m_GamePanelStack;
                break;
            default:
                break;
        }
        //用list管理方便调整顺序
        if (m_currentList == null) m_currentList = new List<UIPanelBase>();

        if (m_currentList.Contains(uIPanel))
        {
            m_currentList.Remove(uIPanel);
        }
        m_currentList.Add(uIPanel);
        OrderSiblingIndex();

    }

    private void PopStack(UIPanelBase uIPanel)
    {
        switch (uIPanel.type)
        {
            case UIParentType.Root:
                m_currentList = m_RootPanelStack;
                break;
            case UIParentType.Main:
                m_currentList = m_MainPanelStack;
                break;
            case UIParentType.Game:
                m_currentList = m_GamePanelStack;
                break;
            default:
                break;
        }
        if (m_currentList.Count == 0) Debug.LogError("Panelstack.Count");
        m_currentList.Remove(uIPanel);//出栈
        OrderSiblingIndex();
    }


    private IEnumerator CR_WaitLoadLevel()
    {
        if (m_titleScreen == null) m_titleScreen = GetUIPanel<TitleScreen>();
        m_titleScreen.OpenPanel();
        m_titleScreen.LoadingScreenIn(true);
        ShowPanel<UITips>();

        Debug.Log("关卡开始加载");

        while (!ScenesUtil.SceneLoaded)
        {
            yield return null;
        }

        yield return null;

        Debug.Log("关卡加载完毕");

        float time = 5 - ScenesUtil.LoadingTime; //tips todo
        if (time > 0)
        {
            yield return new WaitForSecondsRealtime(time);
        }

        m_titleScreen.LoadingScreenOut(true);
        ClosePanel<UITips>();
    }

    private IEnumerator CR_WaitLoadMainScene()
    {
        if (m_titleScreen == null) m_titleScreen = GetUIPanel<TitleScreen>();
        m_titleScreen.OpenPanel();
        m_titleScreen.LoadingScreenIn(false);

        Debug.Log("主场景开始加载");

        while (!ScenesUtil.SceneLoaded)
        {
            yield return null;
        }

        yield return null;

        Debug.Log("主场景加载完毕");

        m_titleScreen.LoadingScreenOut(false);

    }

    #endregion

    #region 事件

    /// <summary>
    /// 游戏结束
    /// </summary>
    /// <param name="status"></param>
    //private void OnGameStatusChanged( GameStatus status)
    //{
    //    switch (status)
    //    {
    //        case GameStatus.EndingSuccess:
    //            ShowPanel<UIEndNormal>();
    //            break;
    //        case GameStatus.EndingFail:
    //            if (LevelManager.Mode == LevelMode.Endless)
    //            {

    //            }
    //            break;
    //    }
    //}

    /// <summary>
    /// UI面板关闭
    /// </summary>
    /// <param name="uIPanel"></param>
    private void OnUIPanelClose(UIPanelBase uIPanel)
    {
        PopStack(uIPanel);
    }

    /// <summary>
    /// UI面板打开
    /// </summary>
    /// <param name="uIPanel"></param>
    private void OnUIPanelOpen(UIPanelBase uIPanel)
    {
        PushStack(uIPanel);
    }
    #endregion
}

#region UIPrefab
[Serializable]
public class UIPrefab
{
    public GameObject prefab;
    public UIParentType type;

}

public enum UIParentType
{
    Root = 0,
    Main = 1,
    Game = 2,
}
#endregion
