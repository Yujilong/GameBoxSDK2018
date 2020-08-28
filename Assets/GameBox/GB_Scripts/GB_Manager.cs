using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class GB_Manager : MonoBehaviour
{
    public static GB_Manager _instance;
    [NonSerialized]
    public static bool NeedAdapterScreen = false;
    public static Vector2 NeedAdapterMoveDownOffset = new Vector2(0, 82);
    private string deviceID = string.Empty;
    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
        NeedAdapterScreen = (float)Screen.width / Screen.height < 9 / 16f;
        deviceID = SystemInfo.deviceUniqueIdentifier;
        gameObject.AddComponent<GB_UIManager>().Init(
            transform.GetChild(0),
            transform.GetChild(1),
            transform.GetChild(2));

        AllAPKnames = GetAllApk();
    }
    private void Start()
    {
        GB_UIManager.Instance.ShowMenuPanel();
        GB_UIManager.Instance.ShowPopPanelAsync(GB_PopPanelType.Privacy);
    }
    const string MenuDataUri = "http://funtapgame.com:7070/api/game/list/device_id";
    IEnumerator ConnectToGetMenuData(Action callback)
    {
        List<IMultipartFormSection> iparams = new List<IMultipartFormSection>();
        iparams.Add(new MultipartFormDataSection("deviceID", deviceID));
        UnityWebRequest www = UnityWebRequest.Get(MenuDataUri);
        yield return www.SendWebRequest();

        while (www.isNetworkError || www.isHttpError )
        {
            Debug.LogError(www.error);
            www.Dispose();
            www = UnityWebRequest.Post(MenuDataUri, iparams);
            yield return www.SendWebRequest();
        }
        // Show results as text
        Debug.Log(www.downloadHandler.text);
        ReceiveData tempData = JsonMapper.ToObject<ReceiveData>(www.downloadHandler.text);
        callback?.Invoke();
    }
    public void SetTexture(string uri,string iconSaveName, Action<Texture2D> callback, bool forceWeb)
    {
        if(forceWeb)
            StartCoroutine(WaitForDownloadTexture(uri, callback));
        else
        {
            string localPath = Application.temporaryCachePath + "/" + iconSaveName + ".jpg";
            if (File.Exists(localPath))
                StartCoroutine(WaitForLoadLocalTexture(localPath, callback));
            else
                StartCoroutine(WaitForDownloadTexture(uri, callback));
        }
    }
    void SetDownloadTexture(string uri, Action<Texture2D> callback)
    {
        StartCoroutine(WaitForDownloadTexture(uri, callback));
    }
    IEnumerator WaitForDownloadTexture(string uri, Action<Texture2D> callback)
    {
        UnityWebRequest wr = new UnityWebRequest(uri);
        DownloadHandlerTexture texDl = new DownloadHandlerTexture(true);
        wr.downloadHandler = texDl;
        yield return wr.SendWebRequest();
        while (wr.isNetworkError || wr.isHttpError)
        {
            Debug.LogError("DownLoad GB_Texture Error : " + wr.error + "////" + uri);
            texDl.Dispose();
            wr.Dispose();
            wr = new UnityWebRequest(uri);
            texDl = new DownloadHandlerTexture(true);
            wr.downloadHandler = texDl;
            yield return wr.SendWebRequest();
        }
        Texture2D t = texDl.texture;
        callback?.Invoke(t);
    }
    void SetLocalTexture(string path, Action<Texture2D> callback)
    {
        StartCoroutine(WaitForLoadLocalTexture(path, callback));
    }
    public IEnumerator WaitForLoadLocalTexture(string path, Action<Texture2D> callback)
    {
        string filePath = "file:///" + path;
        WWW www = new WWW(filePath);
        yield return www;
        if (www.texture is object)
        {
            callback.Invoke(www.texture);
        }
    }
    public static void SaveTexture(Texture2D texture, string fileName)
    {
        byte[] data = texture.EncodeToJPG();
        File.WriteAllBytes(Application.temporaryCachePath + "/" + fileName + ".jpg", data);
    }
    public static List<string> AllAPKnames;
    static List<string> GetAllApk()
    {
        List<string> apks = new List<string>();
#if UNITY_EDITOR

#elif UNITY_ANDROID
        try
        {
            AndroidJavaClass up = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = up.GetStatic<AndroidJavaObject>("currentActivity");
            AndroidJavaObject packageManager = currentActivity.Call<AndroidJavaObject>("getPackageManager");
            AndroidJavaObject packageInfos = packageManager.Call<AndroidJavaObject>("getInstalledPackages", 0);
            AndroidJavaObject[] packages = packageInfos.Call<AndroidJavaObject[]>("toArray");
            for (int i = 0; i < packages.Length; i++)
            {
                AndroidJavaObject applicationInfo = packages[i].Get<AndroidJavaObject>("applicationInfo");
                if ((applicationInfo.Get<int>("flags") & applicationInfo.GetStatic<int>("FLAG_SYSTEM")) == 0)// 判断是不是系统应用
                {
                    string packageName = applicationInfo.Get<string>("packageName");
                    AndroidJavaObject applicationLabel = packageManager.Call<AndroidJavaObject>("getApplicationLabel", applicationInfo);
                    string packageLable = applicationLabel.Call<string>("toString");
                    apks.Add(packageLable + "|" + packageName);
                }
            }
        }
        catch (System.Exception e)
        {
        }
#endif
        return apks;
    }
}
public struct ReceiveData
{
    public int code;
    public ReceiveAppData[] data;
    public string msg;
}
public struct ReceiveAppData
{
    public string create_time;
    public int deleted;
    public string game_brief;
    public string game_detail;
    public string game_download_url;
    public string game_logob;
    public string game_logos;
    public string game_name;
    public string game_pkg_url;
    public int id;
    public bool played;
    public int type;
}
