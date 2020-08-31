using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GB_UI_ShopNoticePanel : GB_UIBase
{
    public Button btn_ok;
    public Button btn_close;
    protected override void Awake()
    {
        base.Awake();
        btn_ok.onClick.AddListener(ClosePanel);
        btn_close.onClick.AddListener(ClosePanel);
    }
    void ClosePanel()
    {
        GB_UIManager.Instance.ClosePopPanelAsync(GB_PopPanelType.ShopNotice);
    }
}
