using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GB_UI_GamePagePanel : GB_UIBase
{
    public GameObject go_singleMyGame;
    public GameObject go_singleNewGame;
    public RectTransform rect_notice;
    public RectTransform rect_games;
    public ScrollRect sr_content;
    public Text text_notice;
    List<GB_UI_MyGameItem> list_allMyGames = new List<GB_UI_MyGameItem>();
    List<GB_UI_NewGameItem> list_allNewGames = new List<GB_UI_NewGameItem>();
    protected override void Awake()
    {
        base.Awake();
        if (GB_Manager.NeedAdapterScreen)
        {
            rect_notice.anchoredPosition -= GB_Manager.NeedAdapterMoveDownOffset;
            rect_games.anchoredPosition -= GB_Manager.NeedAdapterMoveDownOffset/2;
            rect_games.sizeDelta -= GB_Manager.NeedAdapterMoveDownOffset;
        }
    }
    public override IEnumerator OnEnter()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        sr_content.verticalNormalizedPosition = 2;
        yield return null;
    }
    public override IEnumerator OnExit()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        yield return null;
    }
}
