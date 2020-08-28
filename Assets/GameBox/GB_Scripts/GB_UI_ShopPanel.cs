using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class GB_UI_ShopPanel : GB_UIBase
{
    public GameObject go_singleShopItem;
    public RectTransform rect_mask;
    public ScrollRect sr_content;
    readonly List<ShopGoodsConfig> list_goodsConfigs = new List<ShopGoodsConfig>()
    {
        new ShopGoodsConfig(){_GoodsType=ShopGoodsType.Paypal,_GoodsNum=20,_NeedCoinNum=20000},
        new ShopGoodsConfig(){_GoodsType=ShopGoodsType.Paypal,_GoodsNum=30,_NeedCoinNum=30000},
        new ShopGoodsConfig(){_GoodsType=ShopGoodsType.Paypal,_GoodsNum=40,_NeedCoinNum=40000},
        new ShopGoodsConfig(){_GoodsType=ShopGoodsType.Paypal,_GoodsNum=50,_NeedCoinNum=50000},
        new ShopGoodsConfig(){_GoodsType=ShopGoodsType.Paypal,_GoodsNum=100,_NeedCoinNum=100000},
        new ShopGoodsConfig(){_GoodsType=ShopGoodsType.Amazon,_GoodsNum=22,_NeedCoinNum=20000},
        new ShopGoodsConfig(){_GoodsType=ShopGoodsType.Amazon,_GoodsNum=39,_NeedCoinNum=35000},
        new ShopGoodsConfig(){_GoodsType=ShopGoodsType.Amazon,_GoodsNum=59,_NeedCoinNum=55000},
        new ShopGoodsConfig(){_GoodsType=ShopGoodsType.Amazon,_GoodsNum=99,_NeedCoinNum=95000},
        new ShopGoodsConfig(){_GoodsType=ShopGoodsType.Amazon,_GoodsNum=299,_NeedCoinNum=250000}
    };
    readonly List<GB_UI_ShopGoodsItem> list_goodsItems = new List<GB_UI_ShopGoodsItem>();
    struct ShopGoodsConfig
    {
        public ShopGoodsType _GoodsType;
        public int _GoodsNum;
        public int _NeedCoinNum;
    }
    enum ShopGoodsType
    {
        Paypal,
        Amazon,
    }
    SpriteAtlas ShopAtlas;
    Sprite sp_paypal;
    Sprite sp_amazon;
    protected override void Awake()
    {
        base.Awake();
        list_goodsItems.Add(go_singleShopItem.GetComponent<GB_UI_ShopGoodsItem>());
        ShopAtlas = GB_UIManager.Instance.GetSpriteAtlas(GB_FullScreenPanelType.Shop);

        sp_paypal = ShopAtlas.GetSprite("GB_Sprite_Main_Paypal");
        sp_amazon = ShopAtlas.GetSprite("GB_Sprite_Main_Amazon");

        if (GB_Manager.NeedAdapterScreen)
        {
            rect_mask.anchoredPosition -= GB_Manager.NeedAdapterMoveDownOffset / 2;
            rect_mask.sizeDelta -= GB_Manager.NeedAdapterMoveDownOffset;
        }

        int configCount = list_goodsConfigs.Count;
        Transform parent = go_singleShopItem.transform.parent;
        while (list_goodsItems.Count < configCount)
        {
            GameObject newGoods = Instantiate(go_singleShopItem, parent);
            list_goodsItems.Add(newGoods.GetComponent<GB_UI_ShopGoodsItem>());
        }
        string paypal = " USD through  PAYAPAL";
        string amazon = " USD through AMAZON";
        string coins = " Coins";
       for(int i = 0; i < configCount; i++)
        {
            ShopGoodsConfig config = list_goodsConfigs[i];
            if (config._GoodsType == ShopGoodsType.Amazon)
                list_goodsItems[i].Init(sp_amazon, config._GoodsNum + amazon, config._NeedCoinNum + coins, OnWithdrawButtonClick);
            else
                list_goodsItems[i].Init(sp_paypal, config._GoodsNum + paypal, config._NeedCoinNum + coins, OnWithdrawButtonClick);
        }
    }
    void OnWithdrawButtonClick()
    {

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
