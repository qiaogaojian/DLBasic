using DLBasic.Common;
using EXECLConfig;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExcleCfgDatas : ScriptableObject
{
    //��ʼ������
    public void InitData()
    {
        InitLocalData();
    }

    #region ���ػ�����

    [ReadOnly]
    public LocalConfigItem[] localConfigItems;

    private Dictionary<string, Dictionary<string, string>> lanagerDic = new Dictionary<string, Dictionary<string, string>>();

    public void InitLocalData()
    {
        for (int i = 0; i < localConfigItems.Length; i++)
        {
            if (!lanagerDic.ContainsKey(localConfigItems[i].key))
            {
                var temp = localConfigItems[i].stringStruct.ToDictionary(Key => Key.key, Value => Value.value);
                lanagerDic.Add(localConfigItems[i].key, temp);
            }
        }
    }

    public string HasLang(string key)
    {
        if (lanagerDic.ContainsKey(key))
        {
            return key;
        }
        return "English";
    }

    public string GetStrByKey(string key)
    {
        string curLanguage = CommonTool.GetMachineLanage();

        if (lanagerDic[curLanguage].ContainsKey(key))
        {
            return lanagerDic[curLanguage][key];
        }
        XDebug.LogError($"û������Key��{key} ");
        return "";
    }
    #endregion
}
