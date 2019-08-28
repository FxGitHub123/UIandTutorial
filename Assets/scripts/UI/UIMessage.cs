using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class UIMessage
{
    public static Action<UIPanelBase> OnUIPanelOpen = (uipanel) => { };//uipanel打开
    public static Action<UIPanelBase> OnUIPanelClose = (uipanel) => { };//uipanel关闭
}
