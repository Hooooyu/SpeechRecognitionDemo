using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 重写Button类，检测长按事件
/// </summary>
public class ListenButton : Button
{

    public delegate void RecordDelegate();
    public event RecordDelegate OnStartRecordEvent;
    public event RecordDelegate OnStopRecordEvent;

    public PointerEventData currentData;   // 保存一份PointerEventData,方便控制释放鼠标点击事件

    private TipsReference.RECORD_TYPE _TYPE = TipsReference.RECORD_TYPE.None;

    /// <summary>
    /// 释放鼠标长按事件
    /// </summary>
    public void ReleaseClickEvent(TipsReference.RECORD_TYPE type)
    {
        _TYPE = type;
        switch (type)
        {
            case TipsReference.RECORD_TYPE.Normal:
                break;

            case TipsReference.RECORD_TYPE.NoMicroPhone:
                base.OnPointerUp(currentData);
                break;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        currentData = eventData;
        OnStartRecordEvent();
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        if(_TYPE != TipsReference.RECORD_TYPE.NoMicroPhone)
            OnStopRecordEvent();
    }
}
