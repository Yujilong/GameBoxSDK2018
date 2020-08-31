using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D;
using System;

public class GB_UIManager : MonoBehaviour
{
    public static GB_UIManager Instance;
    readonly Stack<GB_UIBase> Panel_Stack = new Stack<GB_UIBase>();
    readonly Dictionary<int, string> Type_Path_Dic = new Dictionary<int, string>()
    {
        {(int)GB_FullScreenPanelType.GamePage,"GB_UIPanel/GB_UI_GamePagePanel" },
        {(int)GB_FullScreenPanelType.Shop,"GB_UIPanel/GB_UI_ShopPanel" },
        {(int)GB_PopPanelType.Help,"GB_UIPanel/GB_UI_HelpPanel" },
        {(int)GB_PopPanelType.Privacy,"GB_UIPanel/GB_UI_PrivacyPanel" },
        {(int)GB_PopPanelType.FirstReward,"GB_UIPanel/GB_UI_FirstRewardPanel" },
    };
    readonly Dictionary<int, GB_UIBase> LoadedPanel_Dic = new Dictionary<int, GB_UIBase>();

    readonly Dictionary<int, string> Type_SAPath_Dic = new Dictionary<int, string>()
    {
        {(int)GB_FullScreenPanelType.Shop,"GB_SpriteAtlas/GB_Main" },
    };
    readonly Dictionary<int, SpriteAtlas> LoadedSpriteAtlas_Dic = new Dictionary<int, SpriteAtlas>();
    const string MenuPanelPath = "GB_UIPanel/GB_UI_MenuPanel";
    GB_UIBase Current_GamePanel = null;
    public GB_UI_MenuPanel MenuPanel = null;
    Transform PopPanelRoot;
    Transform FullScreenPanelRoot;
    Transform MenuPanelRoot;
    public void Init(Transform gamePanelRoot, Transform menuPanelRoot, Transform popPanelRoot)
    {
        PopPanelRoot = popPanelRoot;
        FullScreenPanelRoot = gamePanelRoot;
        MenuPanelRoot = menuPanelRoot;
        Instance = this;
    }
    readonly Queue<PanelTask> Queue_PopPanel = new Queue<PanelTask>();
    Coroutine Cor_PopPanelTask = null;
    public void ShowPopPanelAsync(GB_PopPanelType _PopPanelType)
    {
        PanelTask newTask = new PanelTask()
        {
            t_panelType = _PopPanelType,
            t_open = true
        };
        Queue_PopPanel.Enqueue(newTask);
        if (Cor_PopPanelTask is null)
            Cor_PopPanelTask = StartCoroutine(ExcuteTask());

    }
    public void ClosePopPanelAsync(GB_PopPanelType _PopPanelType)
    {
        PanelTask newTask = new PanelTask()
        {
            t_panelType = _PopPanelType,
            t_open = false
        };
        Queue_PopPanel.Enqueue(newTask);
        if (Cor_PopPanelTask is null)
            Cor_PopPanelTask = StartCoroutine(ExcuteTask());
    }


    IEnumerator ExcuteTask()
    {
        while (Queue_PopPanel.Count > 0)
        {
            PanelTask nextTask = Queue_PopPanel.Dequeue();
            int panelIndex = (int)nextTask.t_panelType;
            bool open = nextTask.t_open;
            if (LoadedPanel_Dic.TryGetValue(panelIndex, out GB_UIBase loadedPopPanel))
            {
                if (loadedPopPanel is null)
                {
                    Debug.LogWarning((open ? "Show" : "Close") + " MG_PopPanel-" + nextTask.t_panelType + " Error : loadedDic has key , but content is null.");
                    continue;
                }
                else
                {
                    if (Panel_Stack.Contains(loadedPopPanel))
                    {
                        if (open)
                        {
                            Debug.LogWarning("Show MG_PopPanel-" + nextTask.t_panelType + " Error : panel has showed.");
                            continue;
                        }
                        else
                        {
                            while (true)
                            {
                                if (Panel_Stack.Count > 0)
                                {
                                    GB_UIBase outPanel = Panel_Stack.Pop();
                                    yield return outPanel.OnExit();
                                    if (Panel_Stack.Count > 0)
                                        Panel_Stack.Peek().OnResume();
                                    else
                                    {
                                        MenuPanel.OnResume();
                                        if (Current_GamePanel is object)
                                            Current_GamePanel.OnResume();
                                    }
                                    if (outPanel == loadedPopPanel)
                                        break;
                                }
                                else
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if (open)
                        {
                            if (Panel_Stack.Count > 0)
                                Panel_Stack.Peek().OnPause();
                            else
                            {
                                MenuPanel.OnPause();
                                if (Current_GamePanel is object)
                                    Current_GamePanel.OnPause();
                            }
                            loadedPopPanel.transform.SetAsLastSibling();
                            Panel_Stack.Push(loadedPopPanel);
                            yield return loadedPopPanel.OnEnter();
                        }
                        else
                        {
                            Debug.LogWarning("Close MG_PopPanel-" + nextTask.t_panelType + " Error : panel has not show.");
                            continue;
                        }
                    }
                }
            }
            else
            {
                if (open)
                {
                    if (Type_Path_Dic.TryGetValue(panelIndex, out string panelPath))
                    {
                        if (string.IsNullOrEmpty(panelPath))
                        {
                            Debug.LogWarning("Show MG_PopPanel-" + nextTask.t_panelType + " Error : panelPathDic content is null or empty.");
                            continue;
                        }
                        else
                        {
                            if (Panel_Stack.Count > 0)
                                Panel_Stack.Peek().OnPause();
                            else
                            {
                                MenuPanel.OnPause();
                                if (Current_GamePanel is object)
                                    Current_GamePanel.OnPause();
                            }
                            GB_UIBase nextShowPanel = Instantiate(Resources.Load<GameObject>(panelPath), PopPanelRoot).GetComponent<GB_UIBase>();
                            nextShowPanel.transform.SetAsLastSibling();
                            Panel_Stack.Push(nextShowPanel);
                            LoadedPanel_Dic.Add(panelIndex, nextShowPanel);
                            yield return nextShowPanel.OnEnter();
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Show MG_PopPanel-" + nextTask.t_panelType + " Error : panelPathDic content is null or empty.");
                        continue;
                    }
                }
                else
                {
                    Debug.LogWarning("Close MG_PopPanel-" + nextTask.t_panelType + " Error : panel has not loaded or show.");
                    continue;
                }
            }
        }
        Cor_PopPanelTask = null;
    }
    public bool CloseTopPopPanelAsync()
    {
        if (Panel_Stack.Count > 0)
        {
            GB_UIBase _UIBase = Panel_Stack.Peek();
            if (LoadedPanel_Dic.ContainsValue(_UIBase))
            {
                foreach (var keyValue in LoadedPanel_Dic)
                {
                    if (keyValue.Value == _UIBase)
                    {
                        ClosePopPanelAsync((GB_PopPanelType)keyValue.Key);
                        return true;
                    }

                }
            }
        }
        return false;
    }
    public bool ShowFullScreenPanel(GB_FullScreenPanelType _PanelType)
    {
        int panelIndex = (int)_PanelType;
        if (LoadedPanel_Dic.TryGetValue(panelIndex, out GB_UIBase loadedGamePanel))
        {
            if (loadedGamePanel is null)
            {
                Debug.LogWarning("Show MG_GamePanel-" + _PanelType + " Error : loadedDic has key , but content is null.");
                return false;
            }
            if (Current_GamePanel == loadedGamePanel)
            {
                //Debug.LogWarning("Show MG_GamePanel-" + _PanelType + " Error : panel has show.");
                return false;
            }
            if (Current_GamePanel is object)
            {
                StartCoroutine(Current_GamePanel.OnExit());
            }
            StartCoroutine(loadedGamePanel.OnEnter());
            Current_GamePanel = loadedGamePanel;
        }
        else
        {
            if (Type_Path_Dic.TryGetValue(panelIndex, out string panelPath))
            {
                if (string.IsNullOrEmpty(panelPath))
                {
                    Debug.LogWarning("Show MG_GamePanel-" + _PanelType + " Error : panelPathDic content is null or empty.");
                    return false;
                }
                GB_UIBase nextShowPanel = Instantiate(Resources.Load<GameObject>(panelPath), FullScreenPanelRoot).GetComponent<GB_UIBase>();
                if (Current_GamePanel is object)
                {
                    StartCoroutine(Current_GamePanel.OnExit());
                }
                nextShowPanel.transform.SetAsLastSibling();
                StartCoroutine(nextShowPanel.OnEnter());
                LoadedPanel_Dic.Add(panelIndex, nextShowPanel);
                Current_GamePanel = nextShowPanel;
            }
            else
            {
                Debug.LogWarning("Show MG_GamePanel-" + _PanelType + " Error : panelPathDic content is null or empty.");
                return false;
            }
        }
        return true;
    }
    public void CloseCurrentFullscreenPanel()
    {
        if (Current_GamePanel is object)
        {
            StartCoroutine(Current_GamePanel.OnExit());
            Current_GamePanel = null;
        }
    }
    public bool ShowMenuPanel()
    {
        if (MenuPanel is null)
        {
            MenuPanel = Instantiate(Resources.Load<GameObject>(MenuPanelPath), MenuPanelRoot).GetComponent<GB_UI_MenuPanel>();
            StartCoroutine(MenuPanel.OnEnter());
            return true;
        }
        Debug.LogWarning("Show MG_MenuPanel Error : panel has show.");
        return false;
    }
    public SpriteAtlas GetSpriteAtlas(int index)
    {
        if (LoadedSpriteAtlas_Dic.TryGetValue(index, out SpriteAtlas loadedSA))
        {
            if (loadedSA is null)
            {
                Debug.LogWarning("Get MG_SpriteAtlas-" + index + " Error : loadedDic has key , but content is null.");
                LoadedSpriteAtlas_Dic.Remove(index);
                return null;
            }
            return loadedSA;
        }
        else
        {
            if (Type_SAPath_Dic.TryGetValue(index, out string path))
            {
                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogWarning("Get MG_SpriteAtlas-" + index + " Error : SAPathDic content is null or empty.");
                    return null;
                }
                SpriteAtlas tempSA = Resources.Load<SpriteAtlas>(path);
                LoadedSpriteAtlas_Dic.Add(index, tempSA);
                return tempSA;
            }
            else
            {
                Debug.LogWarning("Get MG_SpriteAtlas-" + index + " Error : SAPathDic content is null or empty.");
                return null;
            }
        }
    }
    public SpriteAtlas GetSpriteAtlas(GB_FullScreenPanelType fullScreenPanelType)
    {
        return GetSpriteAtlas((int)fullScreenPanelType);
    }
    public GB_UIBase Get_UIPanel(int index)
    {
        if (LoadedPanel_Dic.TryGetValue(index, out GB_UIBase temp))
        {
            return temp;
        }
        else
        {
            Debug.LogWarning("Get MG_UIPanel Error : panel has not show.");
            return null;
        }
    }
    public GB_UIBase Get_UIPanel(GB_FullScreenPanelType _FullScreenPanelType)
    {
        return Get_UIPanel((int)_FullScreenPanelType);
    }
    public GB_UIBase Get_UIPanel(GB_PopPanelType _PopPanelType)
    {
        return Get_UIPanel((int)_PopPanelType);
    }
    public bool HasShow_PopPanel(GB_PopPanelType _PopPanelType)
    {
        int panelIndex = (int)_PopPanelType;
        if (LoadedPanel_Dic.ContainsKey(panelIndex))
        {
            if (Panel_Stack.Contains(LoadedPanel_Dic[panelIndex]))
            {
                return true;
            }
        }
        return false;
    }
    struct PanelTask
    {
        public GB_PopPanelType t_panelType;
        public bool t_open;
    }
}
public enum GB_PopPanelType
{
    Help = 2,
    Privacy = 3,
    FirstReward = 4,
}
public enum GB_FullScreenPanelType
{
    GamePage = 0,
    Shop = 1,
}
