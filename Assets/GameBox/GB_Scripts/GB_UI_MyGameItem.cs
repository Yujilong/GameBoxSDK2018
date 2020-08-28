using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GB_UI_MyGameItem : MonoBehaviour
{
    public RawImage img_icon;
    public Text text_appname;
    public Text text_appdes;
    public Button btn_play;
    public RectTransform rect_loading;
    string app_pkg_name;
    public void Init(string icon_name, string app_name, string app_des, string app_pkg_name)
    {
        text_appname.text = app_name;
        text_appdes.text = app_des;
        this.app_pkg_name = app_pkg_name;
        img_icon.color = Color.white;
        StopCoroutine("Loading");
        rect_loading.gameObject.SetActive(false);
        if (!string.IsNullOrEmpty(icon_name))
        {
            img_icon.color = Color.gray;
            StartCoroutine("Loading");
            GB_Manager._instance.SetTexture(icon_name, (t) =>
            {
                StopCoroutine("Loading");
                img_icon.color = Color.white;
                rect_loading.gameObject.SetActive(false);
                img_icon.texture = t;
                string saveFileName = icon_name.Replace("*", "");
                GB_Manager.SaveTexture(t, saveFileName);
            });
        }
        btn_play.onClick.RemoveAllListeners();
        if (!string.IsNullOrEmpty(app_pkg_name))
            btn_play.onClick.AddListener(OnPlayButtonClick);
    }
    void OnPlayButtonClick()
    {
        if (GB_Manager.AllAPKnames.Contains(app_pkg_name))
        {
            using (AndroidJavaClass jcPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject joActivity = jcPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    using (AndroidJavaObject joPackageManager = joActivity.Call<AndroidJavaObject>("getPackageManager"))
                    {
                        using (AndroidJavaObject joIntent = joPackageManager.Call<AndroidJavaObject>("getLaunchIntentForPackage", app_pkg_name))
                        {
                            if (null != joIntent)
                            {
                                joActivity.Call("startActivity", joIntent);
                            }
                        }
                    }
                }
            }
        }
        else
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=" + app_pkg_name);
        }
    }
    static Vector3 rotateSpeed = new Vector3(0, 0, 100);
    IEnumerator Loading()
    {
        rect_loading.gameObject.SetActive(true);
        while (true)
        {
            yield return null;
            rect_loading.Rotate(rotateSpeed * Time.unscaledDeltaTime);
        }
    }
}
