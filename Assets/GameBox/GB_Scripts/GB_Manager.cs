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
    LocalData m_LocalData;
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
        m_LocalData = GetLocalData();
        AllAPKnames = GetAllApk();
    }
    private void Start()
    {
        GB_UIManager.Instance.ShowMenuPanel();
        CheckNeedGetServerPageData();
        if (m_LocalData.isFirst)
        {
            GB_UIManager.Instance.ShowPopPanelAsync(GB_PopPanelType.Privacy);
        }
    }
    public void OnAgreePrivacy()
    {
        m_LocalData.isFirst = false;
        SaveData();
    }
    public int GetCoinNum()
    {
        return m_LocalData.coin;
    }
    public List<GamePageData> GetAllPageGameData()
    {
        return m_LocalData.data;
    }
    const string BaseDataUri = "http://funtapgame.com:7070/api/device";
    const string GamePageDataUri = "http://funtapgame.com:7070/api/game/list/";
    const string TexturePrefix = "http://funtapgame.com:7070/api/image/";
    public void GetBaseData()
    {
        StartCoroutine("ConnectToGetBaseData");
    }
    IEnumerator ConnectToGetBaseData()
    {
        List<IMultipartFormSection> iparams = new List<IMultipartFormSection>();
        iparams.Add(new MultipartFormDataSection("deviceID", deviceID));
        UnityWebRequest www = UnityWebRequest.Post(BaseDataUri, iparams);
        yield return www.SendWebRequest();

        while (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError(www.error);
            www.Dispose();
            www = UnityWebRequest.Post(GamePageDataUri, iparams);
            yield return www.SendWebRequest();
        }
        // Show results as text
        Debug.Log(www.downloadHandler.text);
        ReceiveRegistryData tempData = JsonMapper.ToObject<ReceiveRegistryData>(www.downloadHandler.text);
        m_LocalData.coin = tempData.data.coin;
        SaveData();
        GB_UIManager.Instance.MenuPanel.UpdateCoinNum();
    }
    IEnumerator ConnectToGetGamePageData()
    {
        UnityWebRequest www = UnityWebRequest.Get(GamePageDataUri + deviceID);
        yield return www.SendWebRequest();

        while (www.isNetworkError || www.isHttpError )
        {
            Debug.LogError(www.error);
            www.Dispose();
            www = UnityWebRequest.Get(GamePageDataUri);
            yield return www.SendWebRequest();
        }
        // Show results as text
        Debug.Log(www.downloadHandler.text);
        ReceiveGamePageData tempData = JsonMapper.ToObject<ReceiveGamePageData>(www.downloadHandler.text);
        int count = tempData.data.Length;
        m_LocalData.data.Clear();
        for(int i = 0; i < count; i++)
            m_LocalData.data.Add(tempData.data[i]);
        SaveData();
    }
    public void SetTexture(string name, Action<Texture2D> callback)
    {
        string localPath = Application.temporaryCachePath + "/" + name + ".jpg";
        if (File.Exists(localPath))
            StartCoroutine(WaitForLoadLocalTexture(localPath, callback));
        else
            StartCoroutine(WaitForDownloadTexture(TexturePrefix + name, callback));
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
        int sideLength = Mathf.Min(t.width, t.height);
        Texture2D cubeT = new Texture2D(sideLength, sideLength);
        int startPosX = t.width / 2 - sideLength / 2;
        int startPosY = t.height / 2 - sideLength / 2;
        for (int width = 0; width < sideLength; width++)
        {
            for (int height = 0; height < sideLength; height++)
            {
                Color color = t.GetPixel(startPosX + width, startPosY + height);
                cubeT.SetPixel(width, height, color);
            }
        }
        cubeT.Apply();
        callback?.Invoke(cubeT);
    }
    IEnumerator WaitForLoadLocalTexture(string path, Action<Texture2D> callback)
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
        fileName = fileName.Substring(0, fileName.Length - 4);
        byte[] data = texture.EncodeToJPG();
        //File.WriteAllBytes(Application.temporaryCachePath + "/" + fileName + ".jpg", data);
    }
    void SaveData()
    {
        if(m_LocalData.data is object)
        {
            PlayerPrefs.SetString("GB_LocalData", JsonMapper.ToJson(m_LocalData));
            PlayerPrefs.Save();
        }
    }
    LocalData GetLocalData()
    {
        string saveStr = PlayerPrefs.GetString("GB_LocalData", string.Empty);
        if (string.IsNullOrEmpty(saveStr))
        {
            return new LocalData() { coin = 0, data = new List<GamePageData>(), isFirst = true, hasReadPrivacy = false, lastGetPageDataDate = DateTime.Now.AddDays(-1) };
        }
        else
            return JsonMapper.ToObject<LocalData>(saveStr);
    }
    void CheckNeedGetServerPageData()
    {
        DateTime lastGetDate = m_LocalData.lastGetPageDataDate;
        DateTime now = DateTime.Now;
        bool needReGet = true;
        if (lastGetDate.Year == now.Year)
        {
            if (lastGetDate.Month == now.Month)
            {
                if (lastGetDate.Day == now.Day)
                {
                    needReGet = false;
                }
            }
        }
        if (needReGet)
        {
            StartCoroutine(ConnectToGetGamePageData());
        }
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
    struct ReceiveRegistryData
    {
        public int code;
        public BaseData data;
        public string msg;
    }
    struct ReceiveGamePageData
    {
        public int code;
        public GamePageData[] data;
        public string msg;
    }
    public struct GamePageData
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
    struct BaseData
    {
        public int coin;
        public string create_time;
        public string device_id;
        public int energy;
        public int id;
        public int interstitial;
        public int reward;
    }
    struct LocalData
    {
        public int coin;
        public bool isFirst;
        public bool hasReadPrivacy;
        public DateTime lastGetPageDataDate;
        public List<GamePageData> data;
    }
}
