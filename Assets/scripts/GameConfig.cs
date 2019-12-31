using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Game Config for all the game settings and configs
/// </summary>
[CreateAssetMenuAttribute(fileName = "GameConfig", menuName= "Create GameConfig", order = 1)]
[Serializable]
public class GameConfig : ScriptableObject
{
    public UIPanelConfig panelConfig;
}




public class BaseConfig<T> where T : BaseConfigItem
{
    public List<T> itemList;

    public T GetItemById(string id)
    {
        if (itemList != null && itemList.Count > 0)
        {
            foreach (var item in itemList)
            {
                if (item.id.Equals(id)) return item;
            }
        }
        return null;
    }

    public T this[string id] { get { return GetItemById(id); } }
}


public class BaseConfigItem
{
    public string id;
}