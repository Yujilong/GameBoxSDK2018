using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GB_UI_NewGameItem : MonoBehaviour
{
    public RawImage img_ad1;
    public RawImage img_ad2;
    public Text text_appname;
    public Text text_des1;
    public Text text_des2;
    public Text text_des3;
    public Button btn_play;
    public RectTransform rect_loading1;
    public RectTransform rect_loading2;
    string app_pkg_name;
    public void Init(string ad1_uri, string ad2_uri, string app_name, string des1,string des2,string des3,string app_pkg_name,bool forceWeb)
    {
        text_appname.text = app_name;
        text_des1.text = des1;
        text_des2.text = des2;
        text_des3.text = des3;
        StopCoroutine("Loading1");
        StopCoroutine("Loading2");
        this.app_pkg_name = app_pkg_name;
        btn_play.onClick.RemoveAllListeners();
        if (!string.IsNullOrEmpty(app_pkg_name))
            btn_play.onClick.AddListener(OnPlayButtonClick);
        if (!string.IsNullOrEmpty(ad1_uri))
        {
            StartCoroutine("Loading1");
            GB_Manager._instance.SetTexture(ad1_uri, app_name + "1", (t) =>
            {
                img_ad1.texture = t;
                StopCoroutine("Loading1");
            }, forceWeb);
        }
        if (!string.IsNullOrEmpty(ad2_uri))
        {
            StartCoroutine("Loading2");
            GB_Manager._instance.SetTexture(ad2_uri, app_name + "2", (t) =>
            {
                img_ad2.texture = t;
                StopCoroutine("Loading2");
            }, forceWeb);
        }
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
    IEnumerator Loading1()
    {
        rect_loading1.gameObject.SetActive(true);
        while (true)
        {
            yield return null;
            rect_loading1.Rotate(rotateSpeed * Time.unscaledDeltaTime);
        }
    }
    IEnumerator Loading2()
    {
        rect_loading2.gameObject.SetActive(true);
        while (true)
        {
            yield return null;
            rect_loading2.Rotate(rotateSpeed * Time.unscaledDeltaTime);
        }
    }
}
