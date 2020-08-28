using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GB_UI_ShopGoodsItem : MonoBehaviour
{
    public Image img_icon;
    public Text text_des;
    public Text text_needCoins;
    public Button btn_withdraw;
    public void Init(Sprite icon,string des,string needCoins,UnityAction buttonEvent)
    {
        img_icon.sprite = icon;
        text_des.text = des;
        text_needCoins.text = needCoins;
        btn_withdraw.onClick.RemoveAllListeners();
        btn_withdraw.onClick.AddListener(buttonEvent);
    }
}
