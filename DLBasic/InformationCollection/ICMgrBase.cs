using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InformationCollection
{

    [Flags]
    public enum IcSdkEnum
    {
        DuLu = 0x001,                    //嘟噜互娱
        AppsFlyer = 0x002,              //AppsFlyer
        BI = 0x004,       //BI
        Bugly = 0x008,         //Bugly
        AppsFlyerProcessing = 0x010,   //AppsFlyer加工版//只有安卓
        DBT = 0x020,            //多比特//只有安卓
        Adjust=0x040,
        TalkingDataGame = 0x080,         //TalkingDataGame
        TalkingDataApp = 0x100,         //TalkingDataApp,   td统计分app和game版本
        Tenjin = 0x200,
        GameAnalytics = 0x400,
        FireBaseAnalytics=0x800,            
        All = 0
    }



    public enum CurrencyEnum
    {
        USD,//美元
        CNY,//人民币
        EUR,//欧元
    }

    public abstract class ICMgrBase
    {
        public abstract IcSdkEnum GetIcSdkEnum();

        public abstract void Init(ICConfigBase iCConfigBase);

        public abstract void SendEvent(string key);

        public abstract void SendEvent(string key, string value);

        public abstract void SendEvent(string eventName, Dictionary<string, string> dict);


        /// <summary>
        /// 指的是内购，真钱买物品，虚拟金币不要走这
        /// </summary>
        /// <param name="commodityName">商品名称</param>
        /// <param name="price">价钱</param>
        /// <param name="currencyEnum">货币类型</param>
        public virtual void BuyEvent(string commodityName, float price, CurrencyEnum currencyEnum = CurrencyEnum.USD)
        {


        }

        public virtual void OnUpdate()
        {

        }

        public virtual void Close()
        {

        }

        public virtual void OnApplicationPause(bool pauseStatus)
        {

        }

    }
}