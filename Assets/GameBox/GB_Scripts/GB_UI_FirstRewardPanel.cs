using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GB_UI_FirstRewardPanel : GB_UIBase
{
    public Button btn_claim;
    protected override void Awake()
    {
        base.Awake();
        btn_claim.onClick.AddListener(OnClaimButonClick);
    }
    void OnClaimButonClick()
    {
        GB_UIManager.Instance.ClosePopPanelAsync(GB_PopPanelType.FirstReward);
    }
}
