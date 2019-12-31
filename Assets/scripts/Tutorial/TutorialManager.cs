using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialManager : SingletonAutocreate<TutorialManager>
{
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

    public void Register(TutorialObj obj)
    {
        if (m_tutorialObjList == null) m_tutorialObjList = new List<TutorialObj>();
        if (m_tutorialObjList.Contains(obj)) return;
        m_tutorialObjList.Add(obj);
    }

    public void SetGuideTarget(TutorialObj target,bool isMask = true)
    {
        if (m_tutorialPanel == null) m_tutorialPanel = UIManager.GetUIPanel<UITutorialPanel>();
        m_tutorialPanel.SetGuideTarget(target, isMask);
        m_tutorialPanel.OpenPanel();
    }

    public void CloseGuide()
    {
        if (m_tutorialPanel == null) m_tutorialPanel = UIManager.GetUIPanel<UITutorialPanel>();
        m_tutorialPanel.CloseGuide();
        //m_tutorialPanel.ClosePanel();
    }

    public void OpenClickMask()
    {
        if (m_tutorialPanel == null) m_tutorialPanel = UIManager.GetUIPanel<UITutorialPanel>();
        m_tutorialPanel.OpenClickMask();
    }

    public void CloseClickMask()
    {
        if (m_tutorialPanel == null) m_tutorialPanel = UIManager.GetUIPanel<UITutorialPanel>();
        m_tutorialPanel.CloseClickMask();
    }
}
