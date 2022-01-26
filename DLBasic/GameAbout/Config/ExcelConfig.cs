using DLBasic.Common;
using Excel;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;

//excel 结构体
namespace EXECLConfig
{

    #region 本地化配置
    [Serializable]
    public class LocalConfigItem
    {
        public string key;
        public List<StringStruct> stringStruct = new List<StringStruct>();

        public void AddStructs(string k, string v)
        {
            StringStruct data = new StringStruct(k, v);
            stringStruct.Add(data);
        }
    }
    [Serializable]
    public class StringStruct
    {
        public string key;
        public string value;
        public StringStruct(string k, string v)
        {
            key = k;
            value = v;
        }
    }
    #endregion
}