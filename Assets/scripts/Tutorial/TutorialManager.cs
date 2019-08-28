using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TutorialManager : Singleton<TutorialManager>
{
    private List<TutorialObj> TutorialObjList;

    private UITutorialPanel TutorialPanel;

    public TutorialObj GetTutorialObj(string name)
    {
        foreach (var item in TutorialObjList)
        {
            if (item.name.Equals(name))
            {
                return item;
            }
        }

        return null;
    }

    public void Register(TutorialObj obj)
    {
        if (TutorialObjList == null) TutorialObjList = new List<TutorialObj>();
        if (TutorialObjList.Contains(obj)) return;
        TutorialObjList.Add(obj);
    }

    public void SetGuideTarget(TutorialObj target)
    {
        if (TutorialPanel == null) TutorialPanel = UIManager.GetUIPanel<UITutorialPanel>();
        TutorialPanel.SetGuideTarget(target);
        TutorialPanel.OpenPanel();
    }

    public void CloseGuide()
    {
        if (TutorialPanel == null) TutorialPanel = UIManager.GetUIPanel<UITutorialPanel>();
        TutorialPanel.CloseGuide();
        TutorialPanel.ClosePanel();
    }
}
