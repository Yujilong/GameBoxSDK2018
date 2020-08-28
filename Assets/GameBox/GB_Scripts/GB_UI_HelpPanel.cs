using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GB_UI_HelpPanel : GB_UIBase
{
    public Button btn_go;
    protected override void Awake()
    {
        base.Awake();
        btn_go.onClick.AddListener(OnGoClick);
    }
    void OnGoClick()
    {
        GB_UIManager.Instance.ClosePopPanelAsync(GB_PopPanelType.Help);
        GB_UIManager.Instance.MenuPanel.OnGameButtonClick();
    }
}
