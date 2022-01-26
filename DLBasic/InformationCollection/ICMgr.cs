
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Type = System.Type;

namespace InformationCollection
{
    public class ICMgr : MonoBehaviour
    {
        List<ICMgrBase> ICBases;

        static ICMgr _ins = null;
        public static ICMgr Creat()
        {
            if (_ins != null) return _ins;

            GameObject go = new GameObject("ICMgr");
            GameObject.DontDestroyOnLoad(go);
            _ins = go.AddComponent<ICMgr>();
            _ins.Init();
            return _ins;
        }

        void Init()
        {
            ICBases = new List<ICMgrBase>();

            var iCConfigBaseObjs = Resources.LoadAll("InformationCollection", typeof(ICConfigBase));

            foreach (var item in iCConfigBaseObjs)
            {
                var iCConfigBase = item as ICConfigBase;
                if (iCConfigBase.isEnabled)
                {
                    var iCMgrBase = CreateInstance<ICMgrBase>(iCConfigBase.IcMgrPath);
                    ICBases.Add(iCMgrBase);
                    iCMgrBase.Init(iCConfigBase);
                }
            }
        }

        public void SendEvent(string key,IcSdkEnum icSdkEnum= IcSdkEnum.All)
        {

            var functionStruct = new FunctionStruct
            {
                key = key,
                icSdkEnum = icSdkEnum
            };
            FiltrationSdk(functionStruct, FunctionStructEnum.SendEvent_KEY);

        }
        
        public void SendEvent(string key,string value, IcSdkEnum icSdkEnum = IcSdkEnum.All)
        {
            var functionStruct = new FunctionStruct
            {
                key = key,
                value = value,
                icSdkEnum = icSdkEnum
            };
            FiltrationSdk(functionStruct, FunctionStructEnum.SendEvent_KEY_VALUE);

        }


        public void SendEvent(string eventName,Dictionary<string,string> dict, IcSdkEnum icSdkEnum = IcSdkEnum.All)
        {

            var functionStruct = new FunctionStruct
            {
                dict = dict,
                icSdkEnum = icSdkEnum,
                eventName= eventName
            };
            FiltrationSdk(functionStruct, FunctionStructEnum.SendEvent_DICT);

        }



        public void BuyEvent(string commodityName, float price, IcSdkEnum icSdkEnum = IcSdkEnum.All, CurrencyEnum currencyEnum = CurrencyEnum.USD)
        {
            var functionStruct = new FunctionStruct
            {
                commodityName = commodityName,
                price= price,
                currencyEnum = currencyEnum,
                icSdkEnum = icSdkEnum
            };
            FiltrationSdk(functionStruct, FunctionStructEnum.BuyEvent);
        }



        public void Close()
        {
            foreach (var item in ICBases)
            {
                item.Close();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            foreach (var item in ICBases)
            {
                item.OnApplicationPause(pause);
            }
        }

        private void OnApplicationQuit()
        {
            Close();
        }


        private void Update()
        {
            foreach (var item in ICBases)
            {
                item.OnUpdate();
            }
        }

        void FiltrationSdk(FunctionStruct functionStruct, FunctionStructEnum functionStructEnum)
        {
            void RemainSdk(ICMgrBase icMgrBase)
            {
                switch (functionStructEnum)
                {
                    case FunctionStructEnum.SendEvent_KEY:
                        icMgrBase.SendEvent(functionStruct.key);
                        break;
                    case FunctionStructEnum.SendEvent_KEY_VALUE:
                        icMgrBase.SendEvent(functionStruct.key,functionStruct.value);
                        break;
                    case FunctionStructEnum.SendEvent_DICT:
                        icMgrBase.SendEvent(functionStruct.eventName,functionStruct.dict);
                        break;
                    case FunctionStructEnum.BuyEvent:
                        icMgrBase.BuyEvent(functionStruct.commodityName,functionStruct.price,functionStruct.currencyEnum);
                        break;
                }
            }

            var icSdkEnum = functionStruct.icSdkEnum;

            foreach (var item in ICBases)
            {
                if (icSdkEnum==(IcSdkEnum.All))
                {
                    RemainSdk(item);
                }
                else if (icSdkEnum.HasFlag(item.GetIcSdkEnum()))
                {
                    RemainSdk(item);
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

        struct FunctionStruct
        {
            public IcSdkEnum icSdkEnum;
            public string key;
            public string value;
            public string eventName;
            public Dictionary<string, string> dict;
            public string commodityName;
            public float price;
            public CurrencyEnum currencyEnum;
        }
        
        enum FunctionStructEnum
        {
            SendEvent_KEY,
            SendEvent_KEY_VALUE,
            SendEvent_DICT,
            BuyEvent
        }



    }
}