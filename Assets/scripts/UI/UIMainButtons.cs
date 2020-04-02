using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMainButtons : UIPanelBase
{
    [SerializeField]
    private GameObject ktplay;

    [SerializeField]
    private GameObject shop;

    [SerializeField]
    private GameObject subscription;

    public override void Init()
    {
        //base.Init();
    }

    public void NextLevel()
    {
        ScenesUtil.LoadLevel("Test");
       
    }

    public void ShowTutorial()
    {
        TutorialManager.SetGuideTarget(TutorialManager.Instance.GetTutorialObj("shop"));
    }

    public void FinishTutorial()
    {
        Debug.Log("Success");
        TutorialManager.Instance.CloseGuide();
        UIManager.ClosePanel<UITutorialPanel>();
    }


}

