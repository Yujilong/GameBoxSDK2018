using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GB_UI_ContinuePanel : GB_UIBase
{
    public Text text_title;
    public Button btn_ok;
    public Button btn_close;
    protected override void Awake()
    {
        base.Awake();
        btn_ok.onClick.AddListener(OnOk);
        btn_close.onClick.AddListener(ClosePanel);
    }
    void ClosePanel()
    {
        GB_UIManager.Instance.ClosePopPanelAsync(GB_PopPanelType.ContinuePlaying);
    }
    void OnOk()
    {
        GB_UIManager.Instance.ClosePopPanelAsync(GB_PopPanelType.ContinuePlaying);
        GB_UIManager.ContinuePanelOkEvent.Invoke();
    }
    public override IEnumerator OnEnter()
    {
        text_title.text = GB_UIManager.ContinuePanelNeedShowTitle;
        yield return base.OnEnter();
    }
}
