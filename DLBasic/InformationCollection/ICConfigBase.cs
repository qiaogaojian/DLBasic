using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InformationCollection
{

    public abstract class ICConfigBase : ScriptableObject
    {
        public bool isEnabled;
        public string appID;
        [Header("服务器网址")]
        public string Url;
        public abstract string IcMgrPath { get; }

    }
}