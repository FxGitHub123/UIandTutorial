using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public abstract class UIPanelBase : MonoBehaviour
{
    [HideInInspector]
    public UIParentType type;

    private CanvasGroup m_CanvasGroup;
   
    private CanvasGroup CurrentCanvasGroup
    {
        get
        {
            if(m_CanvasGroup == null)
            {
                m_CanvasGroup = GetComponent<CanvasGroup>();
            }
            return m_CanvasGroup;
        }
    }

    /// <summary>
    /// awake里添加事件监听
    /// </summary>
    protected void Awake()
    {
        AddListener();
    }

    /// <summary>
    /// 销毁事件监听
    /// </summary>
    protected void OnDestroy()
    {
        RemoveListener();
        ClosePanel();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public abstract void Init();

    protected virtual void AddListener()
    {

    }

    protected virtual void RemoveListener()
    {

    }

    /// <summary>
    /// 打开UI面板
    /// </summary>
    public void OpenPanel()
    {
        CurrentCanvasGroup.alpha = 1.0f;
        CurrentCanvasGroup.blocksRaycasts = true;
        gameObject.SetActive(true);
        UIMessage.OnUIPanelOpen(this);
    }

    /// <summary>
    /// 关闭UI面板
    /// </summary>
    public void ClosePanel()
    {
        gameObject.SetActive(false);
        UIMessage.OnUIPanelClose(this);
    }

    /// <summary>
    /// 设置面板的canvas group alpha值 显隐
    /// </summary>
    /// <param name="switchOn"></param>
    public void SetActiveState(bool switchOn)
    {
        if (switchOn)
        {
            gameObject.SetActive(true);
            CurrentCanvasGroup.alpha = 1.0f;
            CurrentCanvasGroup.blocksRaycasts = true;
        }
        else
        {
            CurrentCanvasGroup.alpha = 0.0f;
            CurrentCanvasGroup.blocksRaycasts = false;
        }
    }


    /// <summary>
    /// 显隐动画
    /// </summary>
    /// <param name="switchOn"></param>
    /// <param name="fadeTime"></param>
    public void SetActiveStateFade(bool switchOn, float fadeTime)
    {
        if (switchOn)
        {
            gameObject.SetActive(true);
            CurrentCanvasGroup.DOFade(1.0f,fadeTime);
            CurrentCanvasGroup.blocksRaycasts = true;

        }
        else
        {
            CurrentCanvasGroup.DOFade(0.0f, fadeTime);
            CurrentCanvasGroup.blocksRaycasts = false;
        }
    }

}