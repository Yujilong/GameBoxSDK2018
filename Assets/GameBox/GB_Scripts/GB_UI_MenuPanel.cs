using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GB_UI_MenuPanel : GB_UIBase
{
    public Button btn_game;
    public Button btn_cashout;
    public Button btn_help;
    public Button btn_back;
    public RectTransform rect_top;
    protected override void Awake()
    {
        base.Awake();
        btn_game.onClick.AddListener(OnGameButtonClick);
        btn_cashout.onClick.AddListener(OnCashoutButtonClick);
        btn_help.onClick.AddListener(OnHelpButtonClick);
        btn_back.onClick.AddListener(OnBackButtoClick);
        if (GB_Manager.NeedAdapterScreen)
            rect_top.anchoredPosition -= GB_Manager.NeedAdapterMoveDownOffset;
        btn_back.gameObject.SetActive(false);
    }
    bool isInGB = false;
    public void OnGameButtonClick()
    {
        OnEnterGB();
        GB_UIManager.Instance.ShowFullScreenPanel(GB_FullScreenPanelType.GamePage);
    }
    void OnCashoutButtonClick()
    {
        OnEnterGB();
        GB_UIManager.Instance.ShowFullScreenPanel(GB_FullScreenPanelType.Shop);
    }
    void OnHelpButtonClick()
    {
        GB_UIManager.Instance.ShowPopPanelAsync(GB_PopPanelType.Help);
    }
    void OnBackButtoClick()
    {
        OnExitGB();
        GB_UIManager.Instance.CloseCurrentFullscreenPanel();
    }
    public override IEnumerator OnEnter()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        yield return null;
    }
    public override IEnumerator OnExit()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        yield return null;
    }
    void OnEnterGB()
    {
        isInGB = true;
        btn_back.gameObject.SetActive(true);
        btn_game.gameObject.SetActive(false);
    }
    void OnExitGB()
    {
        isInGB = false;
        btn_back.gameObject.SetActive(false);
        btn_game.gameObject.SetActive(true);
    }
}
