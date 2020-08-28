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
    public void Init(string ad1_name, string ad2_name, string app_name, string des1,string des2,string des3,string app_pkg_name)
    {
        text_appname.text = app_name;
        text_des1.text = des1;
        text_des2.text = des2;
        text_des3.text = des3;
        img_ad1.color = Color.white;
        img_ad2.color = Color.white;
        StopCoroutine("Loading1");
        StopCoroutine("Loading2");
        rect_loading1.gameObject.SetActive(false);
        rect_loading2.gameObject.SetActive(false);
        this.app_pkg_name = app_pkg_name;
        btn_play.onClick.RemoveAllListeners();
        if (!string.IsNullOrEmpty(app_pkg_name))
            btn_play.onClick.AddListener(OnPlayButtonClick);
        if (!string.IsNullOrEmpty(ad1_name))
        {
            img_ad1.color = Color.gray;
            StartCoroutine("Loading1");
            GB_Manager._instance.SetTexture(ad1_name, (t) =>
            {
                img_ad1.texture = t;
                StopCoroutine("Loading1");
                rect_loading1.gameObject.SetActive(false);
                img_ad1.color = Color.white;
                string saveFileName = ad1_name.Replace("*", "");
                GB_Manager.SaveTexture(t, saveFileName);
            });
        }
        if (!string.IsNullOrEmpty(ad2_name))
        {
            img_ad2.color = Color.gray;
            StartCoroutine("Loading2");
            GB_Manager._instance.SetTexture(ad2_name, (t) =>
            {
                img_ad2.texture = t;
                StopCoroutine("Loading2");
                rect_loading2.gameObject.SetActive(false);
                img_ad2.color = Color.white;
                string saveFileName = ad2_name.Replace("*", "");
                GB_Manager.SaveTexture(t, saveFileName);
            });
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
