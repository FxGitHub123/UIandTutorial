using System;



[Serializable]
public class UIPanelConfig
{
    public UIPanelConfigItem[] uIPanels;
}



[Serializable]
public class UIPanelConfigItem
{
    public int id;
    public string name;
    public string path;
}
