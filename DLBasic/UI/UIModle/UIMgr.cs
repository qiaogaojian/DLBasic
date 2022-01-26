using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using XAsset;

namespace DLBasic.UI
{
    public enum UIType
    {
        HUDUI,
        SceneBack,
        SceneUI,
        WindowUI = 100,
        WarningUI = WindowUI + 100,
        BannerUI = WarningUI + 100,
        LoadingUI = BannerUI + 100,
        DisplayUI = LoadingUI + 100,
        TopUI = DisplayUI + 100,
    }

    public class UIMgr
    {
        public RectTransform CanvasTrans;
        private Dictionary<UIType, Transform> m_UIParentCache = new Dictionary<UIType, Transform>();
        private Dictionary<long, UIBase> m_UIOpening = new Dictionary<long, UIBase>();
        private Dictionary<string, List<UIBase>> m_UIClose = new Dictionary<string, List<UIBase>>();
        private long m_nextGUID;
        private Assets assets;
        private int invLength = 50;//两层UI间隔

        public UIMgr(Assets assets)
        {
            this.assets = assets;
            GameObject canvasGo = GameObject.Find("Canvas");
            if (canvasGo == null)
            {
                XDebug.LogError("UIRoot not found", LogHelper.B13Port);
            }
            CanvasTrans = canvasGo.transform as RectTransform;
            CanvasScaler scale = CanvasTrans.GetComponent<CanvasScaler>();

            if (Screen.width / (float)Screen.height > 16 / 9f)
            {
                scale.matchWidthOrHeight = 1;
            }
            InitUIType();
        }

        private void InitUIType()
        {
            Transform UIMain = CanvasTrans.Find("Main");

            Array UITypeArray = Enum.GetValues(typeof(UIType));
            for (int i = 0; i < UITypeArray.Length; i++)
            {
                UIType uitype = (UIType)UITypeArray.GetValue(i);
                GameObject ui = new GameObject(uitype.ToString());
                ui.AddComponent<RectTransform>();
                ui.layer = 5;
                Transform uiParent = ui.transform;
                uiParent.SetParent(UIMain);
                m_UIParentCache[uitype] = uiParent;

                RectTransform rect = uiParent as RectTransform;
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.pivot = Vector2.one * .5f;
                rect.sizeDelta = Vector2.zero;
                rect.localScale = Vector3.one;
                rect.anchoredPosition3D = Vector3.forward * (invLength - i * invLength);

                if (uitype == UIType.HUDUI || uitype == UIType.SceneBack) continue;
                Canvas canvas = ui.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                int sortingOrder = uitype == UIType.SceneUI ? 0 : (int)uitype;
                canvas.sortingOrder = sortingOrder;
                ui.AddComponent<GraphicRaycaster>();
            }
        }

        public Transform GetUIParent(UIType type)
        {
            if (m_UIParentCache.ContainsKey(type))
            {
                return m_UIParentCache[type];
            }
            return null;
        }

        public long GetNextGUID()
        {
            if (++m_nextGUID == long.MaxValue)
            {
                m_nextGUID = 0;
            }
            return m_nextGUID;
        }

        public GameObject CreateUIByPath(string uiPath, bool onlyOne = true)
        {
            UIBase _ins = null;
            if (__JudgeCacheAndShow(uiPath, onlyOne, out _ins))
            {
                return _ins.gameObject;
            }

            Asset ob = assets.Load<GameObject>(uiPath);
            if (ob.asset == null) return null;
            GameObject go = UnityEngine.Object.Instantiate(ob.asset) as GameObject;
            UIBase ui = HandleUI(go, ob);
            ui.UIPath = uiPath;
            return go;
        }

        public T CreateUIByPath<T>(string uiPath, bool onlyOne = true) where T : UIBase
        {
            GameObject go = CreateUIByPath(uiPath, onlyOne);
            if (go == null) return null;
            return go.GetComponent<UIBase>() as T;
        }

        /// <summary>
        /// 异步获取UI
        /// </summary>
        /// <param name="uiPath">路径</param>
        /// <param name="act">回调</param>
        public void CreateUIByPathAsync(string uiPath, Action<GameObject> act, bool onlyOne = true)
        {
            CreateUIByPathAsyncEx(uiPath, act, onlyOne);
        }

        public void CreateUIByPathAsync<T>(string uiPath, Action<T> act = null, bool onlyOne = true) where T : UIBase
        {
            CreateUIByPathAsyncEx(uiPath, (go) =>
            {
                if (act != null) act(go.GetComponent<UIBase>() as T); ;
                act = null;
            }, onlyOne);
        }

        private Dictionary<string, Action<GameObject>> __loadActionDic = new Dictionary<string, Action<GameObject>>();
        private Dictionary<string, bool> __loadStateDic = new Dictionary<string, bool>();

        private void CreateUIByPathAsyncEx(string uiPath, Action<GameObject> act, bool onlyOne = true)
        {
            UIBase _ins = null;
            if (__JudgeCacheAndShow(uiPath, onlyOne, out _ins))
            {
                if (act != null) act(_ins.gameObject);
                return;
            }

            __AddloadAction(uiPath, act);
            if (!__loadStateDic.ContainsKey(uiPath))
            {
                __loadStateDic[uiPath] = true;
                Asset ob = assets.LoadAsync<GameObject>(uiPath);
                ob.AddCompletedLisenter((asset) =>
                {
                    GameObject go = UnityEngine.Object.Instantiate(ob.asset) as GameObject;

                    UIBase ui = HandleUI(go, asset);
                    ui.UIPath = uiPath;
                    __loadStateDic.Remove(uiPath);
                    if (__loadActionDic.ContainsKey(uiPath))
                    {
                        Action<GameObject> callback = __loadActionDic[uiPath];
                        callback.Invoke(go);
                        __loadActionDic.Remove(uiPath);
                    }
                });
            }
        }

        private void __AddloadAction(string path, Action<GameObject> act)
        {
            if (act == null) return;
            if (!__loadActionDic.ContainsKey(path))
            {
                __loadActionDic[path] = act;
            }
            else
            {
                __loadActionDic[path] += act;
            }
        }

        private bool __JudgeCacheAndShow(string uiPath, bool onlyOne, out UIBase ui)
        {
            ui = null;
            if (onlyOne == true)
            {
                ui = GetUIByPath(uiPath);
                if (ui != null)
                {
                    ui.gameObject.SetActive(true);
                    return true;
                }
            }
            if (m_UIClose.ContainsKey(uiPath) && m_UIClose[uiPath].Count > 0)
            {
                List<UIBase> uilist = m_UIClose[uiPath];
                ui = uilist[uilist.Count - 1];
                HandleUI(ui.gameObject, null);
                m_UIClose[uiPath].RemoveAt(uilist.Count - 1);
                ui.gameObject.SetActive(true);
                return true;
            }
            return false;
        }

        public UIBase GetUIByPath(string uiPath)
        {
            foreach (var kvp in m_UIOpening)
            {
                if (kvp.Value != null && kvp.Value.UIPath == uiPath)
                    return kvp.Value;
            }
            return null;
        }
        public List<UIBase> GetUIListByPath(string uiPath)
        {
            List<UIBase> uIBases = new List<UIBase>();
            foreach (var kvp in m_UIOpening)
            {
                if (kvp.Value != null && kvp.Value.UIPath == uiPath)
                    uIBases.Add(kvp.Value);
            }
            return uIBases;
        }

        public delegate bool UISelector<T>(T ui);

        public void Close(long uiGUID, bool force = false)
        {
            if (m_UIOpening.ContainsKey(uiGUID))
            {
                UIBase ui = m_UIOpening[uiGUID];
                if (ui != null)
                {
                    ui.OnClose();
                    if (ui.CloseType == 0 || force)
                    {
                        GameObject.Destroy(ui.gameObject);
                    }
                    else if (ui.CloseType == 1)
                    {
                        ui.gameObject.SetActive(false);
                        if (!m_UIClose.ContainsKey(ui.UIPath))
                        {
                            m_UIClose.Add(ui.UIPath, new List<UIBase>());
                        }
                        m_UIClose[ui.UIPath].Add(ui);
                    }
                }
            }
            m_UIOpening.Remove(uiGUID);
        }

        public void CloseAll(bool force = false)
        {
            List<long> keys = new List<long>(m_UIOpening.Count);
            foreach (var item in m_UIOpening)
            {
                keys.Add(item.Key);
            }

            for (int i = 0; i < keys.Count; i++)
            {
                Close(keys[i], force);
            }
        }


        public void CloseAll(long whichPath, bool force = false)
        {
            List<long> keys = new List<long>(m_UIOpening.Count);
            foreach (var item in m_UIOpening)
            {
                keys.Add(item.Key);
            }

            for (int i = 0; i < keys.Count; i++)
            {
                if (keys[i] != whichPath)
                {
                    Close(keys[i], force);
                }

            }
        }

        private UIBase HandleUI(GameObject obj, Asset a)
        {
            if (a != null)
            {
                ReleaseAssetOnDestroy.Register(obj, a);
            }
            UIBase ui = obj.GetComponent<UIBase>();
            if (ui == null)
            {
                ui = obj.AddComponent<UIBase>();
            }
            UIType uitype = ui.UIType;

            var rect = obj.GetComponent<RectTransform>();
            SetUIParent(rect, m_UIParentCache[uitype]);
            ui.GUID = GetNextGUID();
            m_UIOpening[ui.GUID] = ui;
            ui.Init(this);
            return ui;
        }

        public void SetUIParent(RectTransform uiRect, Transform parent, bool matchParent = true)
        {
            uiRect.SetParent(parent);
            uiRect.localScale = Vector3.one;
            if (matchParent)
            {
                uiRect.offsetMax = Vector2.zero;
                uiRect.offsetMin = Vector2.zero;
            }

            uiRect.anchoredPosition3D = Vector3.zero;
        }

        //需要音效区分时必须传入float，区分重载
        public UISimpleMsg ShowSimpleMsg(string msg, bool isMoveUp = false)
        {
            if (string.IsNullOrEmpty(msg)) return null;
            UISimpleMsg re = CreateUIByPath(UIResourcePath.UISimpleMsg, false).GetComponent<UISimpleMsg>();
            re.Init(msg, isMoveUp);
            return re;
        }
        public UISimpleMsg ShowSimpleMsg(string msg, int count, bool isMoveUp = false)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                int Count = 0;
                List<UIBase> uIBases = GetUIListByPath(UIResourcePath.UISimpleMsg);
                uIBases.ForEach((a) => { if (((UISimpleMsg)a).msg == msg) Count++; });
                if (Count >= count) return null;
            }
            UISimpleMsg ui = ShowSimpleMsg(msg, 0, isMoveUp);
            return ui;
        }

    }
}