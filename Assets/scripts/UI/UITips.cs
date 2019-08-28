using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITips : UIPanelBase
{

    [SerializeField]
    private List<Sprite> m_TipsList;

    [SerializeField]
    private Animation[] m_animations;

    [SerializeField]
    private Image m_tipsImage;

    private bool m_ShowTips;
    private float m_AnimaTime;
    private int m_animationCount;

    public override void Init()
    {
        m_ShowTips = false;
        m_animationCount = 0;
        for (int i = 0; i < m_animations.Length; i++)
        {
            m_animations[i].gameObject.SetActive(false);
        }
        ShowTips();
    }

    private Sprite GetItemByRandom()
    {
        if (m_TipsList == null || m_TipsList.Count == 0)
        {
            //string levelid = LevelManager.CurrentLevelId;
            //list = new List<TipsConfigItem>();
            //list = Global.Config.tipsConfig.GetLevelTips(levelid);
            Debug.LogError("TipsListCount 0");
        }
        
        int r = Random.Range(0, m_TipsList.Count);

        if (m_TipsList[r] != null)
        {
            return m_TipsList[r];
        }
        return null;
    }

    private void RemoveItem(Sprite id)
    {
        if (m_TipsList == null || m_TipsList.Count == 0) return;

        Sprite temp = null;
        foreach (var item in m_TipsList)
        {
            if (item.Equals(id))
            {
                temp = item;
                break;
            }
        }
        if (temp != null) m_TipsList.Remove(temp);
    }

    private void ShowTips()
    {
        //toto m_list = getlistbylevelid
        Sprite image = GetItemByRandom();
        m_tipsImage.sprite = image;
        m_ShowTips = true;
        RemoveItem(image);
    }

    private void Update()
    {
        if (m_ShowTips)
        {
            m_AnimaTime += Time.deltaTime;
            if (m_AnimaTime >= 0.5f)
            {
                m_AnimaTime = 0;
                m_animations[m_animationCount].gameObject.SetActive(true);
                m_animations[m_animationCount].Play();
                m_animationCount++;
                if (m_animationCount % m_animations.Length == 0)
                {
                    m_animationCount = 0;
                }
            }
        }
    }
}
