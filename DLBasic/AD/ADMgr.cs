using System;
using UnityEngine;
using System.Collections.Generic;

namespace DuLu.AD
{

    public class ADMgr : MonoBehaviour
    {
        static ADMgr _ins = null;
        private List<ADMgrBase> ADBases;
        public bool isNoAD = false;//免广告开关

        public static ADMgr Creat()
        {
            if (_ins != null) return _ins;
            GameObject go = new GameObject("ADMgr");
            DontDestroyOnLoad(go);
            _ins = go.AddComponent<ADMgr>();
            _ins.Init();
            return _ins;
        }


        private void Init()
        {
            ADBases = new List<ADMgrBase>();

            var ADConfigBaseObjs = Resources.LoadAll("ADConfig", typeof(ADConfigBase));

            foreach (var item in ADConfigBaseObjs)
            {
                var ADConfigBase = item as ADConfigBase;
                if (ADConfigBase.isEnabled)
                {
                    var aDMgrBase = CreateInstance<ADMgrBase>(ADConfigBase.AdMgrPath);
                    ADBases.Add(aDMgrBase);
                    aDMgrBase.Init(ADConfigBase);
                }
            }
        }

        T CreateInstance<T>(string fullName)
        {
            var path = fullName;
            var o = Type.GetType(path);
            var obj = Activator.CreateInstance(o, true);
            return (T)obj;
        }

        public void LoadAwardAD()
        {
#if !UNITY_EDITOR
            for (int i = 0; i < ADBases.Count; i++)
            {
                ADBases[i].LoadAwardAD();
            }
#endif
        }

        public void LoadInsertAD()
        {
#if !UNITY_EDITOR
            for (int i = 0; i < ADBases.Count; i++)
            {
                ADBases[i].LoadInsertAD();
            }
#endif
        }

        public void PlayAwardAD(ADStruct aDStruct)
        {
            if (isNoAD)
            {
                if (aDStruct != null) aDStruct.successCallback?.Invoke();
            }
#if !UNITY_EDITOR
            for (int i = 0; i < ADBases.Count; i++)
            {
                ADBases[i].PlayAwardAD(aDStruct);
            }
#endif
        }

        public void PlayInsertAD(ADStruct aDStruct)
        {
            if (isNoAD)
            {
                if (aDStruct != null) aDStruct.successCallback?.Invoke();
            }
#if !UNITY_EDITOR
            for (int i = 0; i < ADBases.Count; i++)
            {
                ADBases[i].PlayInsertAD(aDStruct);
            }
#endif
        }

        public void CreateBannerAD()
        {
#if !UNITY_EDITOR
            for (int i = 0; i < ADBases.Count; i++)
            {
                ADBases[i].CreateBannerAD();
            }
#endif
        }


        public void HideBannerAD()
        {
#if !UNITY_EDITOR
            for (int i = 0; i < ADBases.Count; i++)
            {
                ADBases[i].HideBannerAD();
            }
#endif
        }

        public void ShowBannerAD(ADStruct aDStruct)
        {
#if !UNITY_EDITOR
            for (int i = 0; i < ADBases.Count; i++)
            {
                ADBases[i].ShowBannerAD(aDStruct);
            }
#endif
        }

        /// <summary>
        /// 广告是否准备好
        /// </summary>
        /// <param name="ADIsReady"></param>
        /// <returns></returns>
        public bool ADIsReady(int ADIsReady)
        {
            int redayAD = 0;
            for (int i = 0; i < ADBases.Count; i++)
            {
                bool isreday = ADBases[i].ADIsReady(ADIsReady);
                redayAD += isreday ? 1 : 0;
            }
            return redayAD > 0;
        }
    }
}
