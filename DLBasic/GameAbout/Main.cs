using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    public static Main Ins = null;
    [NonSerialized] public InformationCollection.ICMgr iCMgr = null;
    [NonSerialized] public DuLu.AD.ADMgr aDMgr = null;
    //public LoadAsyncScene loadScenes;
    private void Awake()
    {
        DontDestroyOnLoad(this);
        Ins = this;
        LogHelper.Init(true);//TODO:正式包关闭
        XDebug.Log("**********Main**********", LogHelper.B13Port);
        iCMgr = InformationCollection.ICMgr.Creat();
        aDMgr = DuLu.AD.ADMgr.Creat();
        //AppsFlyerObjectScript.callback = AFCallBack;
    }

//    private void Start()
//    {
//        //iCMgr.SendEvent(IcmgrEventName.flow_Load);
//#if UNITY_EDITOR
//        loadScenes.GoGame();
//#endif
//    }

    //public void AFCallBack()
    //{
    //    AppsFlyerObjectScript.callback = null;
    //    SetStart();
    //}

//    private bool BIInsertADReturn = false;
//    private void SetStart()
//    {
//        var webGet = HttpGet("http://lucky-cube2048-admin.infinities.club/server/country", "");
//        var CurrentRegion = RegionInfo.CurrentRegion;
//        webData = AnalysisJSON(webGet);
//        GetBISwitch();
//        StartGetBIInsertConfig();
//    }

//    private List<float> insertADCfg;
//    private void StartGetBIInsertConfig()
//    {
//        WWWForm form = new WWWForm();
//        form.AddField("secret_key", "o9PB2scVmh7GazkyIvy939X7N4gt4ALP");
//        form.AddField("event_key", $"app_ad_enable_INTER_{Application.version}");
//        StartCoroutine(GetInsertADConfig("http://lucky-cube2048-admin.infinities.club/server/app_conf", form));
//        insertADCfg = new List<float>();
//    }

//    IEnumerator GetInsertADConfig(string url, WWWForm data)
//    {
//        XDebug.Log("GetInsertADConfig", LogHelper.CY);
//        UnityWebRequest request = UnityWebRequest.Post(url, data);
//        yield return request.SendWebRequest();
//        try
//        {
//            bISwitchData = JsonMapper.ToObject<BISwitchData>(request.downloadHandler.text);

//            if (bISwitchData != null)
//            {
//                Debug.Log("GetInsertADConfig:" + bISwitchData.data);

//                var strs = bISwitchData.data.Split('|');
//                for (int i = 0; i < strs.Length; i++)
//                {
//                    insertADCfg.Add(float.Parse(strs[i]));
//                }
//            }
//        }
//        catch (Exception e)
//        {
//            XDebug.Log("GetInsertADConfigFail:" + e.Message, LogHelper.CY);
//            insertADCfg = null;
//        }
//        finally
//        {
//            BIInsertADReturn = true;
//        }
//    }



//    #region WebRequest
//    private WebData webData;
//    public bool IsChina = true;
//    public RegionInfo CurrentRegion;

//    public void GetBISwitch()
//    {
//        WWWForm form = new WWWForm();
//        form.AddField("secret_key", "o9PB2scVmh7GazkyIvy939X7N4gt4ALP");
//        form.AddField("event_key", $"app_ad_enable_{Application.version}");
//        StartCoroutine(WebBIPost("http://lucky-cube2048-admin.infinities.club/server/app_conf", form));
//    }
//    private BISwitchData bISwitchData = null;
//    private bool switchOpen = false;
//    IEnumerator WebBIPost(string url, WWWForm data)
//    {
//        XDebug.Log("WebADBIPostStart", LogHelper.CY);
//        UnityWebRequest request = UnityWebRequest.Post(url, data);
//        yield return request.SendWebRequest();
//        try
//        {
//            bISwitchData = JsonMapper.ToObject<BISwitchData>(request.downloadHandler.text);

//            if (bISwitchData != null)
//            {
//                switchOpen = bISwitchData.data == "1";
//                XDebug.Log("WebADBIPostSeccess:" + bISwitchData.status, LogHelper.CY);
//            }
//            XDebug.Log($"WebADBIPostSeccess:{Application.systemLanguage == SystemLanguage.Chinese}:{Application.systemLanguage == SystemLanguage.ChineseSimplified}:{Application.systemLanguage == SystemLanguage.ChineseTraditional}:{$"{CurrentRegion}" == "CN"}:{ webData == null }:{webData.data.country == "CN" }:{bISwitchData == null}:{bISwitchData.status == 0}:{!switchOpen}:{AppsFlyerObjectScript.IsOrganic}");
//#if UNITY_EDITOR
//            IsChina = false;
//#else
//            if (Application.systemLanguage == SystemLanguage.Chinese ||
//Application.systemLanguage == SystemLanguage.ChineseSimplified ||
//Application.systemLanguage == SystemLanguage.ChineseTraditional || ($"{CurrentRegion}" == "CN") || webData == null || webData.data.country == "CN" || bISwitchData == null || bISwitchData.status == 0 ||!switchOpen|| AppsFlyerObjectScript.IsOrganic)
//            {
//                IsChina = true;
//            }
//            else
//            {
//                IsChina = false;
//            }
//#endif
//        }
//        catch (Exception e)
//        {
//            XDebug.Log("WebError:" + e.Message, LogHelper.CY);
//            IsChina = true;
//        }
//        finally
//        {
//            Debug.Log($"WebADBIPostSeccess:{IsChina}");
//            AppsFlyerObjectScript.isWangZhuan = !IsChina;
//            AppsFlyerObjectScript.insertADCfg = insertADCfg;

//            loadScenes.GoGame();
//        }
//    }
//    public class BISwitchData
//    {
//        public string msg;
//        public int status;
//        public string data;
//    }

//    public string HttpGet(string url, string data)
//    {
//        try
//        {
//            //创建Get请求
//            url = url + (data == "" ? "" : "?") + data;
//            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
//            request.Method = "GET";
//            request.ContentType = "text/html;charset=UTF-8";
//            //接受返回来的数据
//            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
//            Stream stream = response.GetResponseStream();
//            StreamReader streamReader = new StreamReader(stream, Encoding.GetEncoding("utf-8"));
//            string retString = streamReader.ReadToEnd();
//            streamReader.Close();
//            stream.Close();
//            response.Close();
//            return retString;
//        }
//        catch (Exception e)
//        {
//            XDebug.Log(e.Message, LogHelper.CY);
//            return "";
//        }
//    }

//    private WebData AnalysisJSON(string webGet)
//    {
//        try
//        {
//            WebData data = JsonMapper.ToObject<WebData>(webGet);
//            return data;
//        }
//        catch (Exception e)
//        {
//            XDebug.Log("WebBIGetError:" + e.Message, LogHelper.CY);
//            return null;
//        }
//    }
//    [Serializable]
//    private class WebData
//    {
//        public string info;
//        public int status;
//        public CountryData data;
//    }

//    [Serializable]
//    private class CountryData
//    {
//        public string country;
//        public int is_block;
//        //{"info":"ok","status":1,"data":{"country":"US","is_block":0}}
//    }
//    #endregion
}
