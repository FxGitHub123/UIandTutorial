using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialComponent : MonoBehaviour
{
    [SerializeField]
    private List<TutorialObj> tutorialList;

    private void Start()
    {
        foreach (var item in tutorialList)
        {
            TutorialManager.Instance.Register(item);
        }
    }
}

[Serializable]
public class TutorialObj
{
    public string targetName;
    public Image targetImage;
    public Button targetButton;
}

