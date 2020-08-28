using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GB_UI_PrivacyPanel : GB_UIBase
{
    public Button btn_agree;
    protected override void Awake()
    {
        base.Awake();
        btn_agree.onClick.AddListener(OnAgreeButtonClick);
    }
    void OnAgreeButtonClick()
    {
        GB_Manager._instance.OnAgreePrivacy();
        GB_UIManager.Instance.ClosePopPanelAsync(GB_PopPanelType.Privacy);
        GB_UIManager.Instance.ShowPopPanelAsync(GB_PopPanelType.FirstReward);
    }
}
