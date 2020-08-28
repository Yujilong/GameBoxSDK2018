using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GB_UI_GamePagePanel : GB_UIBase
{
    public GameObject go_singleMyGame;
    public GameObject go_singleNewGame;
    public RectTransform rect_notice;
    public RectTransform rect_games;
    public ScrollRect sr_content;
    public Text text_notice1;
    public Text text_notice2;
    RectTransform rect_notice1;
    RectTransform rect_notice2;
    readonly List<GB_UI_MyGameItem> list_allMyGames = new List<GB_UI_MyGameItem>();
    readonly List<GB_UI_NewGameItem> list_allNewGames = new List<GB_UI_NewGameItem>();
    readonly string[] str_tips = new string[]
    {
        "FbGoofball Cashout $20 By PAYPAL!!",
        "UltraFb Cashout $20 By PAYPAL!!",
        "FierceChni Cashout $20 By PAYPAL!!",
        "Keptan Cashout $20 By PAYPAL!!",
        "Suresh Cashout $20 By PAYPAL!!",
        "Reyhan Cashout $20 By PAYPAL!!",
        "GANGSTAT Cashout $20 By PAYPAL!!",
        "abhi Cashout $20 By PAYPAL!!",
        "Javedkhan Cashout $20 By PAYPAL!!",
        "Venus Cashout $20 By PAYPAL!!",
        "Bhavish Cashout $20 By PAYPAL!!",
        "jtn Cashout $20 By PAYPAL!!",
        "Fucek Cashout $20 By PAYPAL!!",
        "Musica Cashout $20 By PAYPAL!!",
        "Rj Cashout $20 By PAYPAL!!",
        "JacKSpaRRoW Cashout $20 By PAYPAL!!",
        "Subham Cashout $20 By PAYPAL!!",
        "Boster Cashout $20 By PAYPAL!!",
        "BesT Cashout $20 By PAYPAL!!",
        "Deepak Cashout $20 By PAYPAL!!",
        "Actionbolt Cashout $20 By PAYPAL!!",
        "Empire Cashout $20 By PAYPAL!!",
        "Badshah Cashout $20 By PAYPAL!!",
        "Yenni Cashout $20 By PAYPAL!!",
        "Hacker Cashout $20 By PAYPAL!!",
        "Protike Cashout $20 By PAYPAL!!",
        "iLOVEyou Cashout $20 By PAYPAL!!",
        "LS Cashout $20 By PAYPAL!!",
        "Burra Cashout $20 By PAYPAL!!",
        "Mrbasu Cashout $20 By PAYPAL!!",
        "Ipin Cashout $20 By PAYPAL!!",
        "afnancjr Cashout $20 By PAYPAL!!",
        "Chimkandi Cashout $20 By PAYPAL!!",
        "Dhaffa Cashout $20 By PAYPAL!!",
        "VIP Cashout $20 By PAYPAL!!",
        "PK Cashout $20 By PAYPAL!!",
        "Raistar Cashout $20 By PAYPAL!!",
        "সত্য Cashout $20 By PAYPAL!!",
        "রাজু নাথ Cashout $20 By PAYPAL!!",
        "PANDEY Cashout $20 By PAYPAL!!",
        "FbGoofball Redeem $20 GiftCard!!",
        "UltraFb Redeem $20 GiftCard!!",
        "FierceChni Redeem $20 GiftCard!!",
        "Keptan Redeem $20 GiftCard!!",
        "Suresh Redeem $20 GiftCard!!",
        "Reyhan Redeem $20 GiftCard!!",
        "GANGSTAT Redeem $20 GiftCard!!",
        "abhi Redeem $20 GiftCard!!",
        "Javedkhan Redeem $20 GiftCard!!",
        "Venus Redeem $20 GiftCard!!",
        "Bhavish Redeem $20 GiftCard!!",
        "jtn Redeem $20 GiftCard!!",
        "Fucek Redeem $20 GiftCard!!",
        "Musica Redeem $20 GiftCard!!",
        "Rj Redeem $20 GiftCard!!",
        "JacKSpaRRoW Redeem $20 GiftCard!!",
        "Subham Redeem $20 GiftCard!!",
        "Boster Redeem $20 GiftCard!!",
        "BesT Redeem $20 GiftCard!!",
        "Deepak Redeem $20 GiftCard!!",
        "Actionbolt Redeem $20 GiftCard!!",
        "Empire Redeem $20 GiftCard!!",
        "Badshah Redeem $20 GiftCard!!",
        "Yenni Redeem $20 GiftCard!!",
        "Hacker Redeem $20 GiftCard!!",
        "Protike Redeem $20 GiftCard!!",
        "iLOVEyou Redeem $20 GiftCard!!",
        "LS Redeem $20 GiftCard!!",
        "Burra Redeem $20 GiftCard!!",
        "Mrbasu Redeem $20 GiftCard!!",
        "Ipin Redeem $20 GiftCard!!",
        "afnancjr Redeem $20 GiftCard!!",
        "Chimkandi Redeem $20 GiftCard!!",
        "Dhaffa Redeem $20 GiftCard!!",
        "VIP Redeem $20 GiftCard!!",
        "PK Redeem $20 GiftCard!!",
        "Raistar Redeem $20 GiftCard!!",
        "সত্য Redeem $20 GiftCard!!",
        "রাজু নাথ Redeem $20 GiftCard!!",
        "PANDEY Redeem $20 GiftCard!!"
    };
    protected override void Awake()
    {
        base.Awake();
        if (GB_Manager.NeedAdapterScreen)
        {
            rect_notice.anchoredPosition -= GB_Manager.NeedAdapterMoveDownOffset;
            rect_games.anchoredPosition -= GB_Manager.NeedAdapterMoveDownOffset / 2;
            rect_games.sizeDelta -= GB_Manager.NeedAdapterMoveDownOffset;
        }
        rect_notice1 = text_notice1.GetComponent<RectTransform>();
        rect_notice2 = text_notice2.GetComponent<RectTransform>();

        list_allMyGames.Add(go_singleMyGame.GetComponent<GB_UI_MyGameItem>());
        list_allNewGames.Add(go_singleNewGame.GetComponent<GB_UI_NewGameItem>());
        List<GB_Manager.GamePageData> allData = GB_Manager._instance.GetAllPageGameData();
        int count = allData.Count;
        int myGameIndex = 0;
        int newGameIndex = 0;
        Transform instantiateParent = go_singleMyGame.transform.parent;
        for (int i = 0; i < count; i++)
        {
            GB_Manager.GamePageData tempData = allData[i];
            if (tempData.played)
            {
                if (list_allMyGames.Count < myGameIndex + 1)
                    list_allMyGames.Add(Instantiate(go_singleMyGame, instantiateParent).GetComponent<GB_UI_MyGameItem>());
                list_allMyGames[myGameIndex].Init(tempData.game_logos, tempData.game_name, tempData.game_brief, tempData.game_pkg_url);
                myGameIndex++;
            }
            else
            {
                if (list_allNewGames.Count < newGameIndex + 1)
                    list_allNewGames.Add(Instantiate(go_singleNewGame, instantiateParent).GetComponent<GB_UI_NewGameItem>());
                string[] adStrs = tempData.game_logob.Split(',');
                list_allNewGames[newGameIndex].Init(adStrs[0], adStrs[1], tempData.game_name, tempData.game_brief, tempData.game_detail, "", tempData.game_pkg_url);
                newGameIndex++;
            }
        }
        if (myGameIndex == 0)
            list_allMyGames[0].gameObject.SetActive(false);
        if (newGameIndex == 0)
            list_allNewGames[0].gameObject.SetActive(false);
        StartCoroutine(AutoMaqueenTips());
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
    IEnumerator AutoMaqueenTips()
    {
        int tipsCount = str_tips.Length;
        int startIndex = 0;
        int countPerNotice = 3;
        string intervalTextPerName = "    ";
        StringBuilder sb_notice1 = new StringBuilder();
        StringBuilder sb_notice2 = new StringBuilder();
        bool willResetNotice1 = true;
        bool willResetNotice2 = true;
        bool firstReset = true;
        float scrollSpeed = 100;
        while (true)
        {
            if (willResetNotice1)
            {
                sb_notice1.Clear();
                for (int i = startIndex; i < startIndex + countPerNotice; i++)
                {
                    int contentIndex = i % tipsCount;
                    sb_notice1.Append(str_tips[contentIndex]);
                    sb_notice1.Append(intervalTextPerName);
                }
                text_notice1.text = sb_notice1.ToString();
                startIndex += countPerNotice;
                willResetNotice1 = false;
                yield return new WaitForEndOfFrame();
                if (firstReset)
                {
                    firstReset = false;
                    rect_notice1.localPosition = Vector2.zero;
                }
                else
                    rect_notice1.localPosition = rect_notice2.localPosition + new Vector3(rect_notice1.sizeDelta.x / 2 + rect_notice2.sizeDelta.x / 2, 0);
            }
            if (willResetNotice2)
            {
                sb_notice2.Clear();
                for (int i = startIndex; i < startIndex + countPerNotice; i++)
                {
                    int contentIndex = i % tipsCount;
                    sb_notice2.Append(str_tips[contentIndex]);
                    sb_notice2.Append(intervalTextPerName);
                }
                text_notice2.text = sb_notice2.ToString();
                startIndex += countPerNotice;
                willResetNotice2 = false;
                yield return new WaitForEndOfFrame();
                rect_notice2.localPosition = rect_notice1.localPosition + new Vector3(rect_notice1.sizeDelta.x / 2 + rect_notice2.sizeDelta.x / 2, 0);
            }
            yield return null;
            Vector3 moveOffset = new Vector3(Time.deltaTime * scrollSpeed, 0);
            rect_notice1.localPosition -= moveOffset;
            rect_notice2.localPosition -= moveOffset;
            if (rect_notice1.localPosition.x <= -(rect_notice1.sizeDelta.x + Screen.width) / 2)
                willResetNotice1 = true;
            if (rect_notice2.localPosition.x <= -(rect_notice2.sizeDelta.x + Screen.width) / 2)
                willResetNotice2 = true;
        }
    }
}
