using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DuLu.AD
{
    public abstract class ADConfigBase : ScriptableObject
    {
        public bool isEnabled;

        public string appID;
        public string rewardADID;
        public string insertADID;
        public string bannerADID;
        public Color bannerBackColor;
        public bool bannerPos;//true =top 

        public string channel;
        public bool isDebug;
        public abstract string AdMgrPath { get; }
    }

}