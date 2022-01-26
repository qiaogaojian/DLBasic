using InformationCollection;
using System.Collections.Generic;

namespace IC.IcDuLu
{
    public class DuLuAnalysisMgr : ICMgrBase
    {
        public override IcSdkEnum GetIcSdkEnum()
        {
            return IcSdkEnum.DuLu;
        }

        public override void Init(ICConfigBase iCConfigBase)
        {
            XDebug.Log("DuLuAnalysisMgr 初始化", LogHelper.B13Port);
        }

        public override void SendEvent(string key)
        {
            XDebug.Log("DuLuAnalysisMgr SendEvent key：" + key, LogHelper.B13Port);
        }

        public override void SendEvent(string key, string value)
        {
            XDebug.Log($"DuLuAnalysisMgr SendEvent key{key} value{value}", LogHelper.B13Port);
        }

        public override void SendEvent(string eventName, Dictionary<string, string> dict)
        {
            XDebug.Log($"DuLuAnalysisMgr SendEvent eventName{eventName}", LogHelper.B13Port);
        }

    }
}