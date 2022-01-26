using System;

namespace DuLu.AD
{

    public enum ADType
    {
        Banner,
        Insert,
        Award
    }

    public abstract class ADMgrBase
    {
        public string bannerNetworkName = "";//banner广告渠道
        public string insertNetworkName = "";//插屏广告渠道
        public string awardNetworkName = "";//激励广告渠道

        public ADStruct playAwardStruct;
        public ADStruct playInsertStruct;
        public ADStruct playBannerStruct;

        public abstract void Init(ADConfigBase aDConfigBase);

        public abstract void LoadAwardAD();

        public abstract void PlayAwardAD(ADStruct aDStruct);

        public abstract void LoadInsertAD();

        public abstract void PlayInsertAD(ADStruct aDStruct);

        public abstract void CreateBannerAD();

        public abstract void ShowBannerAD(ADStruct aDStruct);

        public abstract void HideBannerAD();


        public abstract bool ADIsReady(int type);

        public string Placement(string adInfo, ADType type)
        {
            switch (type)
            {
                case ADType.Banner:
                    return $"{adInfo}_{bannerNetworkName}";
                case ADType.Insert:
                    return $"{adInfo}_{insertNetworkName}";
                case ADType.Award:
                    return $"{adInfo}_{awardNetworkName}";
            }
            return "";
        }
    }

    public class ADStruct
    {
        public string adInfo;
        public Action successCallback;
        public Action failback;
        public Action beginWatchback;
    }

}